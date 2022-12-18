namespace XRNeckSafer
{
    public struct SharedMemoryData
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
}
