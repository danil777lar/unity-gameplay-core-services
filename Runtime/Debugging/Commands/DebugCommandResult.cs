namespace Larje.Core.Debugging
{
    public readonly struct DebugCommandResult
    {
        public readonly bool Success;
        public readonly string Message;
        public readonly object ReturnValue;

        public DebugCommandResult(bool success, string message, object returnValue = null)
        {
            Success = success;
            Message = message;
            ReturnValue = returnValue;
        }

        public static DebugCommandResult Fail(string message)
        {
            return new DebugCommandResult(false, message);
        }
    }
}
