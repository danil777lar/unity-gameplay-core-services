using UnityEngine;

namespace Larje.Core.Debugging
{
    public readonly struct DebugLogEntry
    {
        public readonly float Time;
        public readonly LogType Type;
        public readonly string Message;
        public readonly string StackTrace;

        public DebugLogEntry(float time, LogType type, string message, string stackTrace)
        {
            Time = time;
            Type = type;
            Message = message;
            StackTrace = stackTrace;
        }
    }
}
