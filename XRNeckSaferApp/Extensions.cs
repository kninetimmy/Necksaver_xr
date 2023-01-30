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

        public static string ToDisplayString(this Keys key)
        {
            return $"[Key:{key}]";
        }

        public static string GetCloseReasonMessage(this CloseReason reason)
        {
            switch (reason)
            {
                case CloseReason.WindowsShutDown:
                    return "The operating system is closing all applications before shutting down.";
                case CloseReason.MdiFormClosing:
                    return "The parent form of this multiple document interface (MDI) form is closing.";
                case CloseReason.UserClosing:
                    return "The user is closing the form through the user interface (UI), for example by clicking the Close button on the form window, selecting Close from the window's control menu, or pressing ALT+F4.";
                case CloseReason.TaskManagerClosing:
                    return "The Microsoft Windows Task Manager is closing the application.";
                case CloseReason.FormOwnerClosing:
                    return "The owner form is closing.";
                case CloseReason.ApplicationExitCall:
                    return "The System.Windows.Forms.Application.Exit method of the System.Windows.Forms.Application class was invoked.";
                default:
                    return "The cause of the closure was not defined or could not be determined.";
            }
        }
    }
}
