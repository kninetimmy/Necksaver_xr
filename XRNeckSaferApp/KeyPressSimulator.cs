using System.Windows.Forms;
using WindowsInput;

namespace XRNeckSafer
{
    public static class KeyPressSimulator
    {
        static readonly InputSimulator _simulator = new InputSimulator();

        public static void PressKey(Keys key)
        {
            var vkey = (VirtualKeyCode)key;
            _simulator.Keyboard.KeyDown(vkey);
        }

        public static void ReleaseKey(Keys key)
        {
            var vkey = (VirtualKeyCode)key;
            _simulator.Keyboard.KeyUp(vkey);
        }
    }
}
