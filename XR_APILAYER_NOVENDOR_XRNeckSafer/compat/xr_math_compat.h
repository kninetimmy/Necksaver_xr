#pragma once

#include <DirectXMath.h>
#include <openxr/openxr.h>

namespace xr::math {

struct Pose {
    static XrPosef Identity() {
        return XrPosef{XrQuaternionf{0.f, 0.f, 0.f, 1.f}, XrVector3f{0.f, 0.f, 0.f}};
    }

    static XrPosef Invert(const XrPosef& pose) {
        const DirectX::XMVECTOR orientation = LoadXrQuaternion(pose.orientation);
        const DirectX::XMVECTOR invOrientation = DirectX::XMQuaternionInverse(orientation);

        XrPosef inverted = Identity();
        StoreXrQuaternion(&inverted.orientation, invOrientation);

        const DirectX::XMVECTOR position = LoadXrVector3(pose.position);
        const DirectX::XMVECTOR invPosition = DirectX::XMVector3Rotate(-position, invOrientation);
        StoreXrVector3(&inverted.position, invPosition);

        return inverted;
    }

    static XrPosef Multiply(const XrPosef& lhs, const XrPosef& rhs) {
        const DirectX::XMVECTOR lhsOrientation = LoadXrQuaternion(lhs.orientation);
        const DirectX::XMVECTOR rhsOrientation = LoadXrQuaternion(rhs.orientation);
        const DirectX::XMVECTOR outOrientation = DirectX::XMQuaternionMultiply(lhsOrientation, rhsOrientation);

        const DirectX::XMVECTOR lhsPosition = LoadXrVector3(lhs.position);
        const DirectX::XMVECTOR rhsPosition = LoadXrVector3(rhs.position);
        const DirectX::XMVECTOR outPosition = lhsPosition + DirectX::XMVector3Rotate(rhsPosition, lhsOrientation);

        XrPosef result = Identity();
        StoreXrQuaternion(&result.orientation, outOrientation);
        StoreXrVector3(&result.position, outPosition);
        return result;
    }

private:
    static DirectX::XMVECTOR LoadXrQuaternion(const XrQuaternionf& quaternion) {
        return DirectX::XMVectorSet(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
    }

    static void StoreXrQuaternion(XrQuaternionf* outQuaternion, DirectX::FXMVECTOR quaternion) {
        DirectX::XMFLOAT4 q;
        DirectX::XMStoreFloat4(&q, quaternion);
        outQuaternion->x = q.x;
        outQuaternion->y = q.y;
        outQuaternion->z = q.z;
        outQuaternion->w = q.w;
    }

    static DirectX::XMVECTOR LoadXrVector3(const XrVector3f& vector) {
        return DirectX::XMVectorSet(vector.x, vector.y, vector.z, 0.f);
    }

    static void StoreXrVector3(XrVector3f* outVector, DirectX::FXMVECTOR vector) {
        DirectX::XMFLOAT3 v;
        DirectX::XMStoreFloat3(&v, vector);
        outVector->x = v.x;
        outVector->y = v.y;
        outVector->z = v.z;
    }
};

inline DirectX::XMVECTOR LoadXrQuaternion(const XrQuaternionf& quaternion) {
    return DirectX::XMVectorSet(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
}

inline void StoreXrQuaternion(XrQuaternionf* outQuaternion, DirectX::FXMVECTOR quaternion) {
    DirectX::XMFLOAT4 q;
    DirectX::XMStoreFloat4(&q, quaternion);
    outQuaternion->x = q.x;
    outQuaternion->y = q.y;
    outQuaternion->z = q.z;
    outQuaternion->w = q.w;
}

inline DirectX::XMVECTOR LoadXrVector3(const XrVector3f& vector) {
    return DirectX::XMVectorSet(vector.x, vector.y, vector.z, 0.f);
}

inline void StoreXrVector3(XrVector3f* outVector, DirectX::FXMVECTOR vector) {
    DirectX::XMFLOAT3 v;
    DirectX::XMStoreFloat3(&v, vector);
    outVector->x = v.x;
    outVector->y = v.y;
    outVector->z = v.z;
}

} // namespace xr::math
