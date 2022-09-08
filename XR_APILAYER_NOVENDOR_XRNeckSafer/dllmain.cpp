// SPDX-License-Identifier: Apache-2.0
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// This OpenXR layer demonstrates how to intercept the OpenXR calls to xrLocateViews() in order to alter the FOV based on per-application settings.

#include "pch.h"
#include "math.h"
#include <set>

namespace {
    using namespace xr::math;

    const std::string LayerName = "XR_APILAYER_NOVENDOR_XRNeckSafer";

    // The path where the DLL loads config files and stores logs.
    std::string dllHome;

    // The file logger.
    std::ofstream logStream;

    // Function pointers to interact with the next layers and/or the OpenXR runtime.
    PFN_xrGetInstanceProcAddr nextXrGetInstanceProcAddr = nullptr;
    PFN_xrLocateViews nextXrLocateViews = nullptr;
    PFN_xrLocateSpace nextXrLocateSpace = nullptr;
    PFN_xrCreateSession nextXrCreateSession = nullptr;
    PFN_xrEndFrame nextXrEndFrame = nullptr;

    std::set<XrSpace> isViewSpace;
    std::set<XrSpace> isStageSpace;

    PFN_xrCreateReferenceSpace nextXrCreateReferenceSpace = nullptr;

    XrSpaceLocation centerHmdLocation;
    XrSpaceLocation lastHmdLocation;
    XrVector3f delta;
    XrVector3f trans;

    XrQuaternionf HmdOrientation;
    DirectX::XMVECTOR qYawOffset;

    // float csin, ccos;


    void Log(const char* fmt, ...);

    struct shmVal_s {
        float hmdYawAngle;
        float hmdPitchAngle;
        float yawOffset;
        float pitchOffset;
        float lateralOffset;
        float longitudinalOffset;
        float rightMultiplier;
        float leftMultiplier;
        float upMultiplier;
        float downMultiplier;
        int leftStartAt;
        int rightStartAt;
        int upStartAt;
        int downStartAt;
        bool resetHmdOrientation;
        bool useLinearRotation;
        bool useLinearPitchRotation;
        bool holdLinearRotation;
        bool holdLinearPitchRotation;
        bool hasBeenCentered;
 
    } shmValues;

    std::wstring m_memoryName = L"XRNeckSaferSHM";
    HANDLE m_shmHandler = 0;
    shmVal_s* buffer;

    XrSpace m_LocalSpace{ XR_NULL_HANDLE };
    XrSpace m_ViewSpace{ XR_NULL_HANDLE };
    XrSpace m_StageSpace{ XR_NULL_HANDLE };
    XrSession m_Session{ XR_NULL_HANDLE };

    float holdYawOffsetValue;
    float holdPitchOffsetValue;

    struct EulerAngles {
        float roll, pitch, yaw;
    };

    void toMonitor() {

    }

    EulerAngles ToEulerAngles(XrQuaternionf q) {
        EulerAngles angles;

        // roll (x-axis rotation)
        float sinr_cosp = 2 * (q.w * q.y + q.x * q.z);
        float cosr_cosp = 1 - 2 * (q.x * q.x + q.y * q.y);
        angles.yaw = -std::atan2f(sinr_cosp, cosr_cosp);

        // pitch (y-axis rotation)
        float sinp = 2 * (q.w * q.x - q.z * q.y);
        if (std::fabs(sinp) >= 1)
            angles.pitch = -std::copysignf((float)(M_PI / 2), sinp); // use 90 degrees if out of range
        else
            angles.pitch = -std::asinf(sinp);

        // yaw (z-axis rotation)
        float siny_cosp = 2 * (q.w * q.z + q.x * q.y);
        float cosy_cosp = 1 - 2 * (q.y * q.y + q.z * q.z);
        angles.roll = std::atan2f(siny_cosp, cosy_cosp);

        return angles;
    }

    // Utility logging function.
    void InternalLog(const char* fmt, va_list va)
    {
        char buf[1024];
        _vsnprintf_s(buf, sizeof(buf), fmt, va);
        OutputDebugStringA(buf);
        if (logStream.is_open())
        {
            logStream << buf;
            logStream.flush();
        }
    }

    // General logging function.
    void Log(const char* fmt, ...)
    {
        va_list va;
        va_start(va, fmt);
        InternalLog(fmt, va);
        va_end(va);
    }

    // Debug logging function. Can make things very slow (only enabled on Debug builds).
    void DebugLog(const char* fmt, ...)
    {

#ifdef _DEBUG
        va_list va;
        va_start(va, fmt);
        InternalLog(fmt, va);
        va_end(va);
#endif
    }

    // Overrides the behavior of xrCreateSession().
    XrResult XRNeckSafer_xrCreateSession(
        XrInstance instance,
        const XrSessionCreateInfo* createInfo,
        XrSession* session)
    {
        DebugLog("--> XRNeckSafer_xrCreateSession\n");
        // Call the chain to perform the actual operation.
        const XrResult result = nextXrCreateSession(instance, createInfo, session);

        m_Session = *session;
 
        XrReferenceSpaceCreateInfo referenceSpaceCreateInfo{ XR_TYPE_REFERENCE_SPACE_CREATE_INFO, nullptr };
        referenceSpaceCreateInfo.poseInReferenceSpace = Pose::Identity();
        referenceSpaceCreateInfo.referenceSpaceType = XR_REFERENCE_SPACE_TYPE_LOCAL;
        const XrResult resL = nextXrCreateReferenceSpace(*session, &referenceSpaceCreateInfo, &m_LocalSpace);
        DebugLog("LOCAL space: %d\n", m_LocalSpace);
        referenceSpaceCreateInfo.referenceSpaceType = XR_REFERENCE_SPACE_TYPE_VIEW;
        const XrResult resV = nextXrCreateReferenceSpace(*session, &referenceSpaceCreateInfo, &m_ViewSpace);
        isViewSpace.insert(m_ViewSpace);
        DebugLog("VIEW space: %d\n", m_ViewSpace);
        referenceSpaceCreateInfo.referenceSpaceType = XR_REFERENCE_SPACE_TYPE_STAGE;
        const XrResult resS = nextXrCreateReferenceSpace(*session, &referenceSpaceCreateInfo, &m_StageSpace);
        DebugLog("STAGE space: %d\n", m_StageSpace);

        lastHmdLocation.type = XR_TYPE_SPACE_LOCATION;
        lastHmdLocation.next = nullptr;

        XrSpaceLocation startingLocation;

        const XrResult result2 = nextXrLocateSpace(m_ViewSpace, m_LocalSpace, 0, &startingLocation);
        centerHmdLocation = startingLocation;
        lastHmdLocation = startingLocation;
        holdYawOffsetValue = 0;

        DebugLog("XrLocateSpace for HMD %d\n", result2);

        DebugLog("<-- XRNeckSafer_xrCreateSession %d\n", result);

        return result;
    }

    XrResult XRNeckSafer_xrEndFrame(
        XrSession session,
        const XrFrameEndInfo* frameEndInfo)
    {
        DebugLog("--> XRNeckSafer_xrEndFrame\n");
        const XrResult result = nextXrEndFrame(session, frameEndInfo);

        XrSpaceLocation location;
        location.type = XR_TYPE_SPACE_LOCATION;
        location.next = nullptr;

        const XrResult result2 = nextXrLocateSpace(m_ViewSpace, m_LocalSpace, frameEndInfo->displayTime, &location);
        DebugLog("XrLocateSpace for HMD %d\n", result2);

        if (location.locationFlags & XR_SPACE_LOCATION_ORIENTATION_VALID_BIT) {

            // center button pressed? -> current orientation gets center orientation
            if (buffer->resetHmdOrientation) {
                // EulerAngles centerAngles;
                centerHmdLocation = location;
                buffer->resetHmdOrientation = false;
                shmValues.hasBeenCentered = true;
                buffer->hasBeenCentered = shmValues.hasBeenCentered;
            }

            // refuse to do anything before centering
            if (!shmValues.hasBeenCentered) return result2;

            shmValues.yawOffset = buffer->yawOffset;
            shmValues.pitchOffset = buffer->pitchOffset;
            shmValues.longitudinalOffset = buffer->longitudinalOffset;
            shmValues.lateralOffset = buffer->lateralOffset;
            shmValues.useLinearRotation = buffer->useLinearRotation;
            shmValues.holdLinearRotation = buffer->holdLinearRotation;
            shmValues.useLinearPitchRotation = buffer->useLinearPitchRotation;
            shmValues.holdLinearPitchRotation = buffer->holdLinearPitchRotation;


            //substract center orientation from current orientation to get corrected relative HMD orientation
            const DirectX::XMVECTOR orientation = LoadXrQuaternion(location.pose.orientation);
            const DirectX::XMVECTOR centerOrientation = LoadXrQuaternion(centerHmdLocation.pose.orientation);
            const DirectX::XMVECTOR invertCenterOrientation = DirectX::XMQuaternionConjugate(centerOrientation);
            const DirectX::XMVECTOR substractedOrientation = DirectX::XMQuaternionMultiply(orientation, invertCenterOrientation);
            StoreXrQuaternion(&location.pose.orientation, substractedOrientation);
            StoreXrQuaternion(&HmdOrientation, substractedOrientation);

            EulerAngles angles = ToEulerAngles(location.pose.orientation);
            buffer->hmdYawAngle = angles.yaw * 180.f / (float)M_PI;
            buffer->hmdPitchAngle = angles.pitch * 180.f / (float)M_PI;

            if (shmValues.useLinearRotation) {
                shmValues.leftStartAt = buffer->leftStartAt;
                shmValues.rightStartAt = buffer->rightStartAt;
                shmValues.leftMultiplier = buffer->leftMultiplier;
                shmValues.rightMultiplier = buffer->rightMultiplier;

                if (!shmValues.holdLinearRotation) {
                    bool isright = angles.yaw > 0;
                    float multiplier = isright ? shmValues.rightMultiplier : shmValues.leftMultiplier;
                    int startangle = isright ? shmValues.rightStartAt : shmValues.leftStartAt;
                    float startfrom = startangle * (float)M_PI / 180.f;
                    if (abs(angles.yaw) >= startfrom) {
                        shmValues.yawOffset = shmValues.yawOffset + (abs(angles.yaw) - startfrom) * multiplier * (isright ? 1 : -1);
                    }
                    holdYawOffsetValue = shmValues.yawOffset;
                }
                else {
                    shmValues.yawOffset = holdYawOffsetValue;
                }

                trans = { 0 , 0, 0 };
            }
            else {
                trans = { shmValues.lateralOffset , 0, shmValues.longitudinalOffset };
            }

            if (shmValues.useLinearPitchRotation) {
                shmValues.upStartAt = buffer->upStartAt;
                shmValues.downStartAt = buffer->downStartAt;
                shmValues.upMultiplier = buffer->upMultiplier;
                shmValues.downMultiplier = buffer->downMultiplier;

                if (!shmValues.holdLinearPitchRotation) {
                    bool isup = angles.pitch < 0;
                    float multiplier = isup ? shmValues.upMultiplier : shmValues.downMultiplier;
                    int startangle = isup ? shmValues.upStartAt : shmValues.downStartAt;
                    float startfrom = abs(startangle * (float)M_PI / 180.f);
                    if (abs(angles.pitch) >= startfrom) {
                        shmValues.pitchOffset = shmValues.pitchOffset + (abs(angles.pitch) - startfrom) * multiplier * (isup ? -1 : 1);
                    }
                    holdPitchOffsetValue = shmValues.pitchOffset;
                }
                else {
                    shmValues.pitchOffset = holdPitchOffsetValue;
                }
            }

            // save yaw offset as quaternion for later use
            qYawOffset = DirectX::XMQuaternionRotationRollPitchYaw(0.f, -shmValues.yawOffset, 0.f);

        }
        DebugLog("<-- XRNeckSafer_xrEndFrame %d\n", result);

        return result;
    }

    // Overrides the behavior of xrCreateReferenceSpace().
    XrResult XRNeckSafer_xrCreateReferenceSpace(
        XrSession session,
        const XrReferenceSpaceCreateInfo* createInfo,
        XrSpace* space)
    {
        DebugLog("--> XRNeckSafer_xrCreateReferenceSpace\n");
        // Call the chain to perform the actual operation.
        const XrResult result = nextXrCreateReferenceSpace(session, createInfo, space);
       
        // keep record of all the VIEW spaces of the app
        if (createInfo->referenceSpaceType == XR_REFERENCE_SPACE_TYPE_VIEW) {
            isViewSpace.insert(*space);
            DebugLog("VIEW: %d\n", *space);
        }
        if (createInfo->referenceSpaceType == XR_REFERENCE_SPACE_TYPE_LOCAL) {
            DebugLog("LOCAL: %d\n", *space);
        }
        if (createInfo->referenceSpaceType == XR_REFERENCE_SPACE_TYPE_STAGE) {
            DebugLog("STAGE: %d\n", *space);
            isStageSpace.insert(*space);
        }

        DebugLog("<-- XRNeckSafer_xrCreateReferenceSpace %d\n", result);

        return result;
    }

    // Overrides the behavior of xrLocateSpace()
    XrResult XRNeckSafer_xrLocateSpace(
        XrSpace space,
        XrSpace baseSpace,
        XrTime time,
        XrSpaceLocation* location)
    {
        DebugLog("--> XRNeckSafer_xrLocateSpace\n");
        // Call the chain to perform the actual operation.
        const XrResult result = nextXrLocateSpace(space, baseSpace, time, location);

        bool spaceIsViewSpace = isViewSpace.count(space);
        bool baseSpaceIsViewSpace = isViewSpace.count(baseSpace);
        bool baseSpaceIsStageSpace = isStageSpace.count(baseSpace); // IL2: true, DCS: false

        DebugLog("space: %d  bspace: %d\n", space, baseSpace);

        if (shmValues.yawOffset != 0 || shmValues.pitchOffset != 0) {
            // save current location
            XrVector3f pos = location->pose.position;

            // centerHmdLocation was sampled in LOCAL space 
            // when LocateSpace is requested for STAGE basespace we need to compensate the position
            if (baseSpaceIsStageSpace) {
                pos = pos - centerHmdLocation.pose.position;
            }

            DirectX::XMVECTOR vPitchAxis = { 1.f,0.f,0.f };

            if (spaceIsViewSpace && !baseSpaceIsViewSpace) {

                // we want to rotate around the center of the head
                location->pose.position = { 0, 0, 0 };

                // set yaw offset first, than rotate pitch around the hmd yaw + yaw offset lateral (x) axis
                const DirectX::XMVECTOR qHMD = LoadXrQuaternion(location->pose.orientation);
                const DirectX::XMVECTOR qHMDwithYawOffset = DirectX::XMQuaternionMultiply(qHMD, qYawOffset);
                if (DirectX::XMVector4Length(qHMDwithYawOffset).m128_f32[0] != 0) {
                    vPitchAxis = DirectX::XMVector3Rotate(vPitchAxis, qHMDwithYawOffset);
                }
                const DirectX::XMVECTOR qRotatedPitchOffset = DirectX::XMQuaternionRotationAxis(vPitchAxis, -shmValues.pitchOffset);
                const DirectX::XMVECTOR qHMDwithOffset = DirectX::XMQuaternionMultiply(qHMDwithYawOffset, qRotatedPitchOffset);

                StoreXrQuaternion(&location->pose.orientation, qHMDwithOffset);

                StoreXrVector3(&pos, DirectX::XMVector3Rotate(LoadXrVector3(pos), qYawOffset));

                if (baseSpaceIsStageSpace) {
                    pos = pos + centerHmdLocation.pose.position;
                }

                location->pose.position = pos - trans;
            }


            if (baseSpaceIsViewSpace && !spaceIsViewSpace) {

                //          location->pose.position = { 0, 0, 0 };

                //           rotate pitch around the hmd yaw + yaw offset lateral (x) axis, then set yaw offset 

                //           StoreXrPose(&location->pose,
                //               XMMatrixMultiply(LoadXrPose(location->pose),
                //                   DirectX::XMMatrixRotationRollPitchYaw(shmValues.pitchOffset, 0.f, 0.f)));
                //           StoreXrPose(&location->pose,
                //               XMMatrixMultiply(LoadXrPose(location->pose),
                //                   DirectX::XMMatrixRotationRollPitchYaw(0.f, shmValues.yawOffset, 0.f)));
                //           location->pose.position = pos - trans;
            }

        }
        DebugLog("<-- XRNeckSafer_xrLocateSpace %d\n", result);
        return result;
    }

    // Overrides the behavior of xrLocateViews().
    XrResult XRNeckSafer_xrLocateViews(
        const XrSession session,
        const XrViewLocateInfo* const viewLocateInfo,
        XrViewState* const viewState,
        const uint32_t viewCapacityInput,
        uint32_t* const viewCountOutput,
        XrView* const views)
    {
        DebugLog("--> XRNeckSafer_xrLocateViews\n");
        // Call the chain to perform the actual operation.
        const XrResult result = nextXrLocateViews(session, viewLocateInfo, viewState, viewCapacityInput, viewCountOutput, views);

//        // requested NOT for VIEW space: someone is actually asking for the views in a LOCAL/STAGE space
//        if (!isViewSpace.count(viewLocateInfo->space)) {
//
//            // get already rotated head pose
//            XrSpaceLocation headLocation{ XR_TYPE_SPACE_LOCATION, nullptr };
//            headLocation.type = XR_TYPE_SPACE_LOCATION;
//            headLocation.next = nullptr;
//            XRNeckSafer_xrLocateSpace(m_ViewSpace, viewLocateInfo->space, viewLocateInfo->displayTime, &headLocation);
//
//            // get pose of views in VIEW space
//            XrView v[2]{ {XR_TYPE_VIEW, nullptr}, {XR_TYPE_VIEW, nullptr} };
//            const XrViewLocateInfo vinfo = {
//                XR_TYPE_VIEW_LOCATE_INFO,
//                nullptr,
//                XR_VIEW_CONFIGURATION_TYPE_PRIMARY_STEREO,
//                viewLocateInfo->displayTime,
//                m_ViewSpace
//            };
//            const XrResult result2 = nextXrLocateViews(session, &vinfo, viewState, viewCapacityInput, viewCountOutput, v);
//
//            // rotate the views relative to center of head (base of VIEW space)
//            StoreXrPose(&v[0].pose,
//                XMMatrixMultiply(LoadXrPose(v[0].pose),
//                    DirectX::XMMatrixRotationQuaternion(LoadXrQuaternion(headLocation.pose.orientation))));
//            StoreXrPose(&v[1].pose,
//                XMMatrixMultiply(LoadXrPose(v[1].pose),
//                    DirectX::XMMatrixRotationQuaternion(LoadXrQuaternion(headLocation.pose.orientation))));
//
//            // add rotated eye positions to head position 
//            views[0].pose.position = headLocation.pose.position + v[0].pose.position;
//            views[1].pose.position = headLocation.pose.position + v[1].pose.position;
//            // set eye orientation to rotated eye orientation
//            views[0].pose.orientation = v[0].pose.orientation;
//            views[1].pose.orientation = v[1].pose.orientation;
//        }

        DebugLog("<-- XRNeckSafer_xrLocateViews %d\n", result);

        return result;
    }

    // Entry point for OpenXR calls.
    XrResult XRNeckSafer_xrGetInstanceProcAddr(
        const XrInstance instance,
        const char* const name,
        PFN_xrVoidFunction* const function)
    {
        DebugLog("--> XRNeckSafer_xrGetInstanceProcAddr \"%s\"\n", name);

        // Call the chain to resolve the next function pointer.
        const XrResult result = nextXrGetInstanceProcAddr(instance, name, function);
        if (result == XR_SUCCESS)
            if (result == XR_SUCCESS)
            {
            const std::string apiName(name);

            // Intercept the calls handled by our layer.
            if (apiName == "xrLocateViews") {
                nextXrLocateViews = reinterpret_cast<PFN_xrLocateViews>(*function);
                *function = reinterpret_cast<PFN_xrVoidFunction>(XRNeckSafer_xrLocateViews);
            }
            if (apiName == "xrLocateSpace") {
                nextXrLocateSpace = reinterpret_cast<PFN_xrLocateSpace>(*function);
                *function = reinterpret_cast<PFN_xrVoidFunction>(XRNeckSafer_xrLocateSpace);
            }
            if (apiName == "xrCreateSession") {
                nextXrCreateSession = reinterpret_cast<PFN_xrCreateSession>(*function);
                *function = reinterpret_cast<PFN_xrVoidFunction>(XRNeckSafer_xrCreateSession);
            }
            if (apiName == "xrEndFrame") {
                nextXrEndFrame = reinterpret_cast<PFN_xrEndFrame>(*function);
                *function = reinterpret_cast<PFN_xrVoidFunction>(XRNeckSafer_xrEndFrame);
            }
            if (apiName == "xrCreateReferenceSpace") {
                *function = reinterpret_cast<PFN_xrVoidFunction>(XRNeckSafer_xrCreateReferenceSpace);
            }
            
            // Leave all unhandled calls to the next layer.
        }

        DebugLog("<-- XRNeckSafer_xrGetInstanceProcAddr %d\n", result);

        return result;
    }

    // Entry point for creating the layer.
    XrResult XRNeckSafer_xrCreateApiLayerInstance(
        const XrInstanceCreateInfo* const instanceCreateInfo,
        const struct XrApiLayerCreateInfo* const apiLayerInfo,
        XrInstance* const instance)
    {
        DebugLog("--> XRNeckSafer_xrCreateApiLayerInstance\n");

        if (!apiLayerInfo ||
            apiLayerInfo->structType != XR_LOADER_INTERFACE_STRUCT_API_LAYER_CREATE_INFO ||
            apiLayerInfo->structVersion != XR_API_LAYER_CREATE_INFO_STRUCT_VERSION ||
            apiLayerInfo->structSize != sizeof(XrApiLayerCreateInfo) ||
            !apiLayerInfo->nextInfo ||
            apiLayerInfo->nextInfo->structType != XR_LOADER_INTERFACE_STRUCT_API_LAYER_NEXT_INFO ||
            apiLayerInfo->nextInfo->structVersion != XR_API_LAYER_NEXT_INFO_STRUCT_VERSION ||
            apiLayerInfo->nextInfo->structSize != sizeof(XrApiLayerNextInfo) ||
            apiLayerInfo->nextInfo->layerName != LayerName ||
            !apiLayerInfo->nextInfo->nextGetInstanceProcAddr ||
            !apiLayerInfo->nextInfo->nextCreateApiLayerInstance)
        {
            Log("xrCreateApiLayerInstance validation failed\n");
            return XR_ERROR_INITIALIZATION_FAILED;
        }

        // Store the next xrGetInstanceProcAddr to resolve the functions no handled by our layer.
        nextXrGetInstanceProcAddr = apiLayerInfo->nextInfo->nextGetInstanceProcAddr;

        // Call the chain to create the instance.
        XrApiLayerCreateInfo chainApiLayerInfo = *apiLayerInfo;
        chainApiLayerInfo.nextInfo = apiLayerInfo->nextInfo->next;
        const XrResult result = apiLayerInfo->nextInfo->nextCreateApiLayerInstance(instanceCreateInfo, &chainApiLayerInfo, instance);

        // doing this here because we need xrCreateReferenceSpace before it is intercepted
        PFN_xrVoidFunction function = NULL;
        const XrResult result2 = nextXrGetInstanceProcAddr(*instance, "xrCreateReferenceSpace", &function);
        nextXrCreateReferenceSpace = reinterpret_cast<PFN_xrCreateReferenceSpace>(function);
        function = reinterpret_cast<PFN_xrVoidFunction>(XRNeckSafer_xrCreateReferenceSpace);

        DebugLog("<-- XRNeckSafer_xrCreateApiLayerInstance %d\n", result);

        return result;
    }
}

extern "C" {

    // Entry point for the loader.
    XrResult __declspec(dllexport) XRAPI_CALL XRNeckSafer_xrNegotiateLoaderApiLayerInterface(
        const XrNegotiateLoaderInfo* const loaderInfo,
        const char* const apiLayerName,
        XrNegotiateApiLayerRequest* const apiLayerRequest)
    {
        DebugLog("--> (early) XRNeckSafer_xrNegotiateLoaderApiLayerInterface\n");

        // Retrieve the path of the DLL.
        if (dllHome.empty())
        {
            HMODULE module;
            if (GetModuleHandleExA(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS | GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT, (LPCSTR)&dllHome, &module))
            {
                char path[_MAX_PATH];
                GetModuleFileNameA(module, path, sizeof(path));
                dllHome = std::filesystem::path(path).parent_path().string();
            }
            else
            {
                // Falling back to loading config/writing logs to the current working directory.
                DebugLog("Failed to locate DLL\n");
            }            
        }

        // Start logging to file.
        if (!logStream.is_open())
        {
            std::string logFile = (std::filesystem::path(getenv("LOCALAPPDATA")) / std::filesystem::path(LayerName + ".log")).string();
            logStream.open(logFile, std::ios_base::ate);
            Log("dllHome is \"%s\"\n", dllHome.c_str());
        }

        DebugLog("--> XRNeckSafer_xrNegotiateLoaderApiLayerInterface\n");

        if (apiLayerName && apiLayerName != LayerName)
        {
            Log("Invalid apiLayerName \"%s\"\n", apiLayerName);
            return XR_ERROR_INITIALIZATION_FAILED;
        }

        if (!loaderInfo ||
            !apiLayerRequest ||
            loaderInfo->structType != XR_LOADER_INTERFACE_STRUCT_LOADER_INFO ||
            loaderInfo->structVersion != XR_LOADER_INFO_STRUCT_VERSION ||
            loaderInfo->structSize != sizeof(XrNegotiateLoaderInfo) ||
            apiLayerRequest->structType != XR_LOADER_INTERFACE_STRUCT_API_LAYER_REQUEST ||
            apiLayerRequest->structVersion != XR_API_LAYER_INFO_STRUCT_VERSION ||
            apiLayerRequest->structSize != sizeof(XrNegotiateApiLayerRequest) ||
            loaderInfo->minInterfaceVersion > XR_CURRENT_LOADER_API_LAYER_VERSION ||
            loaderInfo->maxInterfaceVersion < XR_CURRENT_LOADER_API_LAYER_VERSION ||
            loaderInfo->maxInterfaceVersion > XR_CURRENT_LOADER_API_LAYER_VERSION ||
            loaderInfo->maxApiVersion < XR_CURRENT_API_VERSION ||
            loaderInfo->minApiVersion > XR_CURRENT_API_VERSION)
        {
            Log("xrNegotiateLoaderApiLayerInterface validation failed\n");
            return XR_ERROR_INITIALIZATION_FAILED;
        }

        // Setup our layer to intercept OpenXR calls.
        apiLayerRequest->layerInterfaceVersion = XR_CURRENT_LOADER_API_LAYER_VERSION;
        apiLayerRequest->layerApiVersion = XR_CURRENT_API_VERSION;
        apiLayerRequest->getInstanceProcAddr = reinterpret_cast<PFN_xrGetInstanceProcAddr>(XRNeckSafer_xrGetInstanceProcAddr);
        apiLayerRequest->createApiLayerInstance = reinterpret_cast<PFN_xrCreateApiLayerInstance>(XRNeckSafer_xrCreateApiLayerInstance);

        // prepare SHM
        m_shmHandler = OpenFileMapping(FILE_MAP_ALL_ACCESS, FALSE, m_memoryName.c_str());

        if (m_shmHandler) {
            Log("XRNeckSafer shared memory found\n");
        }
        else {
            m_shmHandler = CreateFileMapping(
                INVALID_HANDLE_VALUE,
                NULL,
                PAGE_READWRITE,
                0,
                sizeof(shmValues),
                m_memoryName.c_str());

            Log("XRNeckSafer shared memory created\n");
        }

       if (m_shmHandler) {
            buffer = (shmVal_s*)MapViewOfFile(m_shmHandler, FILE_MAP_ALL_ACCESS, 0, 0, sizeof(shmValues));
            if (NULL != buffer) {
                Log("XRNeckSafer shared memory ready\n");
                buffer->hasBeenCentered = false;
            }
            else {
                Log("Cannot map XRNeckSafer shared memory: null buffer.\n");
            }
       }
        else {
            Log("Couldn't create XRNeckSafer shared memory\n");
        }



        DebugLog("<-- XRNeckSafer_xrNegotiateLoaderApiLayerInterface\n");

        Log("%s layer is active\n", LayerName.c_str());

        return XR_SUCCESS;
    }

}
