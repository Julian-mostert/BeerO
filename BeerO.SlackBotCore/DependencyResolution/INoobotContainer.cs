using BeerO.SlackBotCore.MessagingPipeline.Middleware;
using BeerO.SlackBotCore.Plugins;
using StructureMap;

namespace BeerO.SlackBotCore.DependencyResolution
{
    public interface INoobotContainer
    {
        INoobotCore GetNoobotCore();
        IPlugin[] GetPlugins();
        T GetPlugin<T>() where T : class, IPlugin;
        IMiddleware GetMiddlewarePipeline();
        IContainer GetStructuremapContainer();
    }
}