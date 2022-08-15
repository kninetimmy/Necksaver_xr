using SharpDX;
using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Silk.NET.Core.Native;
using Silk.NET.OpenXR;
namespace XRNeckSafer
{
    public class VRStuff
    {
        MemoryMappedFile shm;
        MemoryMappedViewAccessor accessor;
        public shmVal_s shmValues;

        public struct shmVal_s
        {
            public float hmdYawAngle;
            public float hmdPitchAngle;
            public float yawOffset;
            public float pitchOffset;
            public float lateralOffset;
            public float longitudinalOffset;
            public float rightMultiplier;
            public float leftMultiplier;
            public float upMultiplier;
            public float downMultiplier;
            public int leftStartAt;
            public int rightStartAt;
            public int upStartAt;
            public int downStartAt;
            public bool resetHmdOrientation;
            public bool useLinearRotation;
            public bool useLinearPitchRotation;
            public bool holdLinearRotation;
            public bool holdLinearPitchRotation;
            public bool hasBeenCentered;
        }

        public VRStuff()
        {
            string shmName = "XRNeckSaferSHM";
            int shmSize = 80;
            shm = MemoryMappedFile.CreateOrOpen(shmName, shmSize);
            accessor = shm.CreateViewAccessor();

        }

        public unsafe List<String> ListApiLayers()
        {
            // taken from OpenXR toolkit without knowing what I'm doing
            List<String> LayerNameList = new List<String>();
            AssemblyName assemblyName = new AssemblyName();

            AppDomain dom = AppDomain.CreateDomain("temporaryXr");
            try
            {
                // Load the OpenXR package into a temporary app domain. This is so make sure that the registry is read everytime when looking for implicit API layer.

                assemblyName.CodeBase = typeof(XR).Assembly.Location;
                Assembly assembly = dom.Load(assemblyName);
                Type localXR = assembly.GetType("Silk.NET.OpenXR.XR");

                XR xr = (XR)localXR.GetMethod("GetApi").Invoke(null, null);

                // Make sure our layer is installed.
                uint layerCount = 0;
                xr.EnumerateApiLayerProperties(ref layerCount, null);
                var layers = new ApiLayerProperties[layerCount];
                for (int i = 0; i < layers.Length; i++)
                {
                    layers[i].Type = StructureType.TypeApiLayerProperties;
                }
                var layersSpan = new Span<ApiLayerProperties>(layers);
                if (xr.EnumerateApiLayerProperties(ref layerCount, layersSpan) == Silk.NET.OpenXR.Result.Success)
                {
                    bool found = false;
                    for (int i = 0; i < layers.Length; i++)
                    {
                        fixed (void* nptr = layers[i].LayerName)
                        {
                            string layerName = SilkMarshal.PtrToString(new System.IntPtr(nptr));
                            LayerNameList.Add(layerName);
                            if (layerName == "XR_APILAYER_NOVENDOR_XRNeckSafer")
                            {
                                found = true;
                            }
                        }
                    }
                    if (!found)
                    {
                        LayerNameList.Add("\n--> XRNeckSafer API Layer NOT active! <--");
                    }
                }
                else
                {
                    MessageBox.Show("Unable to query API layers\nUse OpenXR developer tools to \nverify layer installation", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LayerNameList.Clear();
                    LayerNameList.Add("Error");
                }
            }
            catch (Exception e)
            {
                string a = e.ToString();

                MessageBox.Show(a+"Unable to query API layers\nUse OpenXR developer tools to \nverify layer installation", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LayerNameList.Clear();
                LayerNameList.Add("Error");
            }
            finally
            {
                AppDomain.Unload(dom);
            }
            return LayerNameList;

        }
        public void resetHmdOrientation()
        {
            shmValues.resetHmdOrientation = true;
            accessor.Write<shmVal_s>(0, ref shmValues);
        }
        public void updateHmdOrientation()
        {
            accessor.Read<shmVal_s>(0, out shmValues);
        }

        public bool HmdWasCentered()
        {
            return shmValues.hasBeenCentered;
        }
        public float getHmdYaw()
        {
            return shmValues.hmdYawAngle;
        }
        public float getHmdPitch()
        {
            return shmValues.hmdPitchAngle;
        }
        public void setLinearRotationSettings(bool uselinear, int leftstart, int rightstart, int leftmult, int rightmult)
        {
            shmValues.useLinearRotation = uselinear;
            shmValues.leftStartAt = leftstart;
            shmValues.rightStartAt = rightstart;
            shmValues.leftMultiplier = (float)leftmult / 100f; 
            shmValues.rightMultiplier = (float)rightmult / 100f;
            accessor.Write<shmVal_s>(0, ref shmValues);
        }
        public void setPitchLinearRotationSettings(bool usepitchlinear, int upstart, int downstart, int upmult, int downmult)
        {
            shmValues.useLinearPitchRotation = usepitchlinear;
            shmValues.upStartAt = upstart;
            shmValues.downStartAt = downstart;
            shmValues.upMultiplier = (float)upmult / 100f;
            shmValues.downMultiplier = (float)downmult / 100f;
            accessor.Write<shmVal_s>(0, ref shmValues);
        }

        public void setOffset(int a, int b, Vector3 trans)
        {
            shmValues.yawOffset = (float)(a * Math.PI / 180);
            shmValues.pitchOffset = (float)(-b * Math.PI / 180);
            shmValues.lateralOffset = trans.X;
            shmValues.longitudinalOffset = trans.Z;
            accessor.Write<shmVal_s>(0, ref shmValues);
        }

        public void setLinearHold(bool h)
        {
            shmValues.holdLinearRotation = h;
            accessor.Write<shmVal_s>(0, ref shmValues);
        }

        public void setPitchLinearHold(bool h)
        {
            shmValues.holdLinearRotation = h;
            accessor.Write<shmVal_s>(0, ref shmValues);
        }
    }
}
