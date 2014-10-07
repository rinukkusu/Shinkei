
namespace Shinkei
{
    public interface IPluginData
    {
        string Name { get; }
        string Version { get; }
        string Author { get; }
    }


    public interface IPlugin
    {
        bool IsEnabled();

        void RegisterEvents(IRC.Eventsink Eventdata);
    }
}
