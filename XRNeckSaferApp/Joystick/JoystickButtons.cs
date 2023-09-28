using System.Collections.Generic;
using System.Text;

namespace XRNeckSafer
{
    public class JoystickButtons: List<JoystickButton>
    {
        public JoystickButtons Clone()
        {
            var clone = new JoystickButtons();
            clone.AddRange(this);
            return clone;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var button in this)
            {
                if (builder.Length > 0)
                {
                    builder.Append("+");
                }
                builder.Append(button.ToString());
            }
            return builder.ToString();
        }
    }
}
