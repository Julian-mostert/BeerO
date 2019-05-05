using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BeerO.SlackBotCore.Configuration;
using BeerO.SlackBotCore.MessagingPipeline.Middleware;
using BeerO.SlackBotCore.MessagingPipeline.Middleware.StandardMiddleware;
using BeerO.SlackBotCore.Plugins.StandardPlugins;
using Microsoft.Extensions.Logging;
using StructureMap;
using StructureMap.Pipeline;

namespace BeerO.SlackBotCore.DependencyResolution
{
    public class ContainerFactory : IContainerFactory
    {
        private readonly IConfigReader _configReader;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        private readonly Type[] _singletons =
        {
            typeof(INoobotCore),
            typeof(NoobotCore),
            typeof(IConfigReader)
        };

        public ContainerFactory(IConfiguration configuration, IConfigReader configReader, ILogger logger = null)
        {
            this._configuration = configuration;
            this._configReader = configReader;
            this._logger = logger;
        }

        public INoobotContainer CreateContainer()
        {
            Registry registry = CreateRegistry();

            this.SetupSingletons(registry);
            this.SetupMiddlewarePipeline(registry);
            Type[] pluginTypes = this.SetupPlugins(registry);

            registry.For<INoobotCore>().Use(x => x.GetInstance<NoobotCore>());
            registry.For<ILogger>().Use(() => this._logger);
            registry.For<IConfigReader>().Use(() => this._configReader);

            INoobotContainer container = CreateContainer(pluginTypes, registry);
            return container;
        }

        private static Registry CreateRegistry()
        {
            var registry = new Registry();

            // setups DI for everything in BeerO.SlackBotCore.SlackBotCore.Core
            registry.Scan(x =>
            {
                x.TheCallingAssembly();
                x.WithDefaultConventions();
            });

            return registry;
        }

        private static INoobotContainer CreateContainer(Type[] pluginTypes, Registry registry)
        {
            var container = new NoobotContainer(pluginTypes);
            registry.For<INoobotContainer>().Use(x => container);
            container.Initialise(registry);
            return container;
        }

        private void SetupSingletons(Registry registry)
        {
            foreach (Type type in this._singletons)
            {
                registry.For(type).Singleton();
            }
        }

        private void SetupMiddlewarePipeline(Registry registry)
        {
            Stack<Type> pipeline = this.GetPipelineStack();

            registry.Scan(x =>
            {
                x.WithDefaultConventions();

                // scan assemblies that we are loading pipelines from
                foreach (Type middlewareType in pipeline)
                {
                    x.AssemblyContainingType(middlewareType);
                }
            });

            registry.For<IMiddleware>().Use<UnhandledMessageMiddleware>();

            if (this._configReader.AboutEnabled)
            {
                registry.For<IMiddleware>().DecorateAllWith<AboutMiddleware>();
            }

            if (this._configReader.StatsEnabled)
            {
                registry.For<IMiddleware>().DecorateAllWith<StatsMiddleware>();
            }

            while (pipeline.Any())
            {
                Type nextType = pipeline.Pop();
                var nextDeclare = registry.For<IMiddleware>();

                // using reflection as Structuremap doesn't allow passing types in at the moment :-(
                MethodInfo decorateMethod = nextDeclare.GetType().GetMethod("DecorateAllWith", new[] { typeof(Func<Instance, bool>) });
                MethodInfo generic = decorateMethod.MakeGenericMethod(nextType);
                generic.Invoke(nextDeclare, new object[] { null });
            }

            if (this._configReader.HelpEnabled)
            {
                registry.For<IMiddleware>().DecorateAllWith<HelpMiddleware>();
            }

            registry.For<IMiddleware>().DecorateAllWith<BeginMessageMiddleware>();
        }

        private Stack<Type> GetPipelineStack()
        {
            Type[] pipelineList = this._configuration.ListMiddlewareTypes() ?? new Type[0];

            var pipeline = new Stack<Type>();
            foreach (Type type in pipelineList)
            {
                pipeline.Push(type);
            }

            return pipeline;
        }

        private Type[] SetupPlugins(Registry registry)
        {
            var pluginTypes = new List<Type>
            {
                typeof(StatsPlugin)
            };

            Type[] customPlugins = this._configuration.ListPluginTypes() ?? new Type[0];
            pluginTypes.AddRange(customPlugins);

            registry.Scan(x =>
            {
                // scan assemblies that we are loading pipelines from
                foreach (Type pluginType in pluginTypes)
                {
                    x.AssemblyContainingType(pluginType);
                    x.WithDefaultConventions();
                }
            });

            // make all plugins singletons
            foreach (Type pluginType in pluginTypes)
            {
                registry
                    .For(pluginType)
                    .Singleton();
            }

            return pluginTypes.ToArray();
        }
    }
}