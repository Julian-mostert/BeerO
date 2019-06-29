using System;
using BeerO.Bot.Config;
using Topshelf;

namespace BeerO.Bot
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