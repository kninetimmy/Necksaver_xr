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
            if (keysToCheck == null || keysToCheck.Length == 0)
            {
                return false;
            }
            var distinctKeysToCheck = keysToCheck.Distinct();
            if (pressedKeys.Count() != distinctKeysToCheck.Count())
            {
                return false;
            }
            return distinctKeysToCheck.All(k => pressedKeys.Contains(k));
        }
    }
}
