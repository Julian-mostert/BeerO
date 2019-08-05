using System;
using System.Collections.Generic;
using BeerO.SlackCore.MessagingPipeline.Middleware;
using BeerO.SlackCore.Plugins;

namespace BeerO.SlackCore.Configuration
{
    public class ConfigurationBase : IConfiguration
    {
        private readonly List<Type> _pipeline = new List<Type>();
        private readonly List<Type> _plugins = new List<Type>();

        public Type[] ListMiddlewareTypes()
        {
            return this._pipeline.ToArray();
        }

        public Type[] ListPluginTypes()
        {
            return this._plugins.ToArray();
        }

        protected void UseMiddleware<T>() where T : IMiddleware
        {
            this._pipeline.Add(typeof(T));
        }

        protected void UsePlugin<T>() where T : IPlugin
        {
            this._plugins.Add(typeof(T));
        }
    }
}