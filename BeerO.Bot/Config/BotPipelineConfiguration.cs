using BeerOBot.ConsoleApp.SlackMiddleWare;
using BeerOBot.ConsoleApp.SlackPlugin;
using Noobot.Core.Configuration;
//using Noobot.Toolbox.Middleware;
//using Noobot.Toolbox.Plugins;

namespace BeerOBot.ConsoleApp.Config
{
    public class BotPipelineConfiguration : ConfigurationBase
    {
        public BotPipelineConfiguration()
        {
            //UseMiddleware<WelcomeMiddleware>();   
            //this.UseMiddleware<PingMiddleware>();
            this.UseMiddleware<SlackTest>();
            this.UseMiddleware<SpotifyMiddleWare>();
            this.UseMiddleware<UnhandledMessageMiddleware>();
            this.UsePlugin<SpotifyPlugin>();
            //this.UsePlugin<JsonStoragePlugin>();
            //UsePlugin<SchedulePlugin>();
            //this.UsePlugin<PingPlugin>();
        }
    }
}