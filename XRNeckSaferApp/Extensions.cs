using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public static bool InDesignerMode(this Control o)
        {
            return Assembly.GetCallingAssembly() != Assembly.GetEntryAssembly();
        }
    }
}
