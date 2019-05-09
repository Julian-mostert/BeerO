using System;
using BeerOBot.ConsoleApp.SlackMiddleWare;
using Noobot.Core.Configuration;

namespace BeerOBot.ConsoleApp.Config
{
    public class Config : ConfigurationBase
    {
        public Config()
        {
            this.UseMiddleware<UnhandledMessageMiddleware>();
        }
    }

    public class SlackConfiguration : IConfigReader
    {
        public bool   HelpEnabled  => true;
        public bool   StatsEnabled => false;
        public bool   AboutEnabled => false;
        public string SlackApiKey  => BotSettings.SlackApiKey;

        public T GetConfigEntry<T>(string entryName)
        {
            throw new NotImplementedException();
        }
    }
}