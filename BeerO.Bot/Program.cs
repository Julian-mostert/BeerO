using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BeerOBot.ConsoleApp.Config;
using BeerOBot.ConsoleApp.Spotify;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Noobot.Core;
using Noobot.Core.Configuration;
using Noobot.Core.DependencyResolution;
using Topshelf;

namespace BeerOBot.ConsoleApp
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting Noobot...");
            try
            {
                HostFactory.Run(x =>
                {
                    x.Service<BotHost>(s =>
                    {
                        s.ConstructUsing(name => new BotHost(new SlackConfiguration()));

                        s.WhenStarted(n => { n.Start(); });

                        s.WhenStopped(n => n.Stop());
                    });

                    x.RunAsNetworkService();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

    }
}