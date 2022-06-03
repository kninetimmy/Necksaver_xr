using SharpDX;
using System;
using System.IO.MemoryMappedFiles;
using System.Threading.Tasks;
using System.Windows.Forms;
using Valve.VR;

namespace XRNeckSafer
{
    public class VRStuff
    {
        MemoryMappedFile shm;
        public shmVal_s shmValues;
        public struct shmVal_s
        {
            public float hmdYawAngle;
            public float hmdPitchAngle;
            public float yawOffset;
            public float pitchOffset;
            public float lateralOffset;
            public float longitudinalOffset;
            public bool resetHmdOrientation;
        }

        public VRStuff()
        {
            string shmName = "XRNeckSaferSHM";
            int shmSize = 80;
            shm = MemoryMappedFile.CreateOrOpen(shmName, shmSize);
        }

        public void resetHmdOrientation()
        {
            MemoryMappedViewAccessor accessor = shm.CreateViewAccessor();
            shmValues.resetHmdOrientation = true;
            accessor.Write<shmVal_s>(0, ref shmValues);
        }
        public void updateHmdOrientation()
        {
            MemoryMappedViewAccessor accessor = shm.CreateViewAccessor();
            accessor.Read<shmVal_s>(0, out shmValues);
        }

        public float getHmdYaw()
        {
            return shmValues.hmdYawAngle;
        }
        public float getHmdPitch()
        {
            return shmValues.hmdPitchAngle;
        }

        public void setOffset(int a, Vector3 trans)
        {
            shmValues.yawOffset = (float)(a * Math.PI / 180);
            shmValues.lateralOffset = trans.X;
            shmValues.longitudinalOffset = trans.Z;
            MemoryMappedViewAccessor accessor = shm.CreateViewAccessor();
            accessor.Write<shmVal_s>(0, ref shmValues);
        }

    }
}
