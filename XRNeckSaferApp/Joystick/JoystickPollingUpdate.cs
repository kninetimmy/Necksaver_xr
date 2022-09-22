using SharpDX.DirectInput;
using System.Globalization;

namespace XRNeckSafer
{
    public class JoystickPollingUpdate
    {
        public int RawOffset { get; set; }

        public int Value { get; set; }

        public JoystickOffset Offset => (JoystickOffset)RawOffset;

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Offset: {0}, Value: {1}", Offset, Value);
        }
    }
}
