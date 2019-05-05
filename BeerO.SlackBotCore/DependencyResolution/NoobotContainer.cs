using System;
using System.Collections.Generic;
using BeerO.SlackBotCore.MessagingPipeline.Middleware;
using BeerO.SlackBotCore.Plugins;
using StructureMap;

namespace BeerO.SlackBotCore.DependencyResolution
{
    internal class NoobotContainer : INoobotContainer
    {
        private Container _container;
        private readonly Type[] _pluginTypes;

        public NoobotContainer(Type[] pluginTypes)
        {
            this._pluginTypes = pluginTypes;
        }

        public void Initialise(Registry registry)
        {
            this._container = new Container(registry);
        }

        public INoobotCore GetNoobotCore()
        {
            return this._container.GetInstance<INoobotCore>();
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