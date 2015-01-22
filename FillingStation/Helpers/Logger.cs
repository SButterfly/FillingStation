using System;
using System.Diagnostics;

namespace FillingStation.Helpers
{
    public static class Logger
    {
        public static void WriteLine(object tag, string message)
        {
            var now = DateTime.Now;
            Debug.WriteLine(now.ToString("O") + " [" + (tag != null ? tag.ToString() : "<null>") + "] " + message);
        }

        public static void WriteLine(object tag, string format, params object[] objects)
        {
            WriteLine(tag, string.Format(format, objects));
        }
    }
}
