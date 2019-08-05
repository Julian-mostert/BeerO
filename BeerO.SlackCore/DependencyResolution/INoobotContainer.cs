using BeerO.SlackCore.MessagingPipeline.Middleware;
using BeerO.SlackCore.Plugins;
using Lamar;

namespace BeerO.SlackCore.DependencyResolution
{
    public interface INoobotContainer
    {
        ISlackBotCore GetNoobotCore();
        IPlugin[] GetPlugins();
        T GetPlugin<T>() where T : class, IPlugin;
        IMiddleware GetMiddlewarePipeline();
        IContainer GetStructuremapContainer();
    }
}