using BeerO.Bot.SlackMiddleWare;
using BeerO.Bot.SlackPlugin;
using Noobot.Core.Configuration;

//using Noobot.Toolbox.Middleware;
//using Noobot.Toolbox.Plugins;

namespace BeerO.Bot.Config
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