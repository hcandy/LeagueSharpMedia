namespace Yasuo.Common.Classes
{
    public interface IChild
    {
        bool Enabled { get; }
        string Name { get; }
        bool Initialized { get; }
        bool Unloaded { get; }
        bool Handled { get; }
        void HandleEvents();
    }
}
