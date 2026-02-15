using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public static class Extensions
    {
        public static bool CheckMatch(this IEnumerable<Keys> pressedKeys, params Keys[] keysToCheck)
        {
            if (pressedKeys == null || keysToCheck == null || keysToCheck.Length == 0)
            {
                return false;
            }

            var pressedSet = new HashSet<Keys>(pressedKeys);
            var checkSet = new HashSet<Keys>(keysToCheck);

            if (pressedSet.Count != checkSet.Count)
            {
                return false;
            }

            return checkSet.All(pressedSet.Contains);
        }
    }
}
