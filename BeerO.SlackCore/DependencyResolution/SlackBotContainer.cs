using System;
using System.Collections.Generic;
using BeerO.SlackCore.MessagingPipeline.Middleware;
using BeerO.SlackCore.Plugins;
using Lamar;
using Microsoft.Extensions.DependencyInjection;

namespace BeerO.SlackCore.DependencyResolution
{
    internal class SlackBotContainer : INoobotContainer
    {
        private Container _container;
        private readonly Type[] _pluginTypes;

        public SlackBotContainer(Type[] pluginTypes)
        {
            this._pluginTypes = pluginTypes;
        }

        public void Initialise(IServiceCollection registry)
        {
            this._container = new Container(registry);
        }

        public ISlackBotCore GetNoobotCore()
        {
            return this._container.GetInstance<ISlackBotCore>();
        }
                
        private IPlugin[] _plugins;
        public IPlugin[] GetPlugins()
        {
            if (this._plugins == null)
            {
                var result = new List<IPlugin>(this._pluginTypes.Length);

                foreach (Type pluginType in this._pluginTypes)
                {
                    IPlugin plugin = this._container.GetInstance(pluginType) as IPlugin;
                    if (plugin == null)
                    {
                        throw new NullReferenceException($"Plugin failed to build {pluginType}");
                    }

                    result.Add(plugin);
                }

                this._plugins = result.ToArray();
            }

            return this._plugins;
        }

        public T GetPlugin<T>() where T : class, IPlugin
        {
            return this._container.TryGetInstance(typeof(T)) as T;
        }

        public IMiddleware GetMiddlewarePipeline()
        {
            return this._container.GetInstance<IMiddleware>();
        }

        public IContainer GetStructuremapContainer()
        {
            return this._container;
        }
    }
}