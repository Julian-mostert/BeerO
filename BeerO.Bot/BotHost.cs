using System;
using System.Threading;
using BeerO.Bot.Config;
using BeerO.Bot.Spotify;
using BeerO.SlackCore;
using BeerO.SlackCore.Configuration;
using BeerO.SlackCore.DependencyResolution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BeerO.Bot
{
    public class BotHost
    {
        private                 IConfigReader    _configReader;
        private                 IConfiguration   _configuration;
        private                 ISlackBotCore      _slackBotCore;
        private static readonly ManualResetEvent _quitEvent = new ManualResetEvent(false);

        public BotHost(IConfigReader configReader)
        {
            this._configReader = configReader;
            this._configuration = new BotPipelineConfiguration();
        }

        public void Start()
        {
            AppDomain.CurrentDomain.ProcessExit += this.ProcessExitHandler;
            Console.CancelKeyPress += this.ConsoleOnCancelKeyPress;

            IServiceCollection serviceBuilder = new ServiceCollection()
                .AddSingleton<ISpotifyBase, SpotifyBase>();

            ILogger test = this.GetLogger(serviceBuilder);
            ContainerFactory containerFactory =
                new ContainerFactory(this._configuration, this._configReader, test);

            INoobotContainer container = containerFactory.CreateContainer();
            this._slackBotCore = container.GetNoobotCore();


            this._slackBotCore
                .Connect()
                .ContinueWith(task =>
                {
                    if (!task.IsCompleted || task.IsFaulted)
                    {
                        Console.WriteLine($"Error connecting to Slack: {task.Exception}");
                    }
                })
                .GetAwaiter()
                .GetResult();

            _quitEvent.WaitOne();


            //serviceBuilder.BuildServiceProvider();
        }

        public void Stop()
        {
            Console.WriteLine("Disconnecting...");
            this._slackBotCore?.Disconnect();
        }

        private ILogger GetLogger(IServiceCollection serviceBuilder)
        {
            serviceBuilder.AddLogging(logging => { logging.AddConsole(); });
            ServiceProvider serviceProvider = serviceBuilder.BuildServiceProvider();

            return serviceProvider.GetRequiredService<ILogger<BotHost>>();
        }

        private void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs consoleCancelEventArgs)
        {
            _quitEvent.Set();
            consoleCancelEventArgs.Cancel = true;
        }

        // not hit
        private void ProcessExitHandler(object sender, EventArgs e)
        {
            Console.WriteLine("Disconnecting...");
            this._slackBotCore?.Disconnect();
        }
    }
}