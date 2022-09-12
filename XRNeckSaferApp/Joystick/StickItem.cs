using SharpDX.DirectInput;

namespace XRNeckSafer
{
    public class StickItem
    {
        public Joystick Stick { get; set; }
        public bool Attached { get; set; }
        public string Guid { get; set; }
        
        public string GetInstanceName()
        {
            return Stick?.Properties?.InstanceName;
        }
    }
}
