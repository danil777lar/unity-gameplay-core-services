namespace Larje.Core.Debugging
{
    public interface IDebugConsoleService
    {
        bool IsEnabled { get; }
        bool IsOpen { get; }

        void Open();
        void Close();
        void Toggle();
    }
}
