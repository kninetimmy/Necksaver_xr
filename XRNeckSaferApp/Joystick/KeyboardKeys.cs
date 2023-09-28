using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public class KeyboardKeys : List<Keys>
    {
        public KeyboardKeys Clone()
        {
            var clone = new KeyboardKeys();
            clone.AddRange(this);
            return clone;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var key in this)
            {
                if (builder.Length > 0)
                {
                    builder.Append("+");
                }
                builder.Append(key.ToDisplayString());
            }
            return builder.ToString();
        }
    }
}
