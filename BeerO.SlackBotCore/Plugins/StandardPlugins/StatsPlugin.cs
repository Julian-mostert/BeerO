using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace BeerO.SlackBotCore.Plugins.StandardPlugins
{
    public class StatsPlugin : IPlugin
    {
        private readonly ILogger _logger;
        private readonly Dictionary<string, object> _stats = new Dictionary<string, object>();
        private readonly object _lock = new object();

        public StatsPlugin(ILogger logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// Sets a key to the given value
        /// </summary>
        public void RecordStat(string key, object value)
        {
            key = key.ToLower();

            lock (this._lock)
            {
                this._stats[key] = value;
            }
        }

        /// <summary>
        /// Assumes current stat is a number and increments it by 1. Defaults to 1 if key doesn't exist.
        /// </summary>
        public void IncrementState(string key)
        {
            key = key.ToLower();

            lock (this._lock)
            {
                int? value2Store = null;
                if (this._stats.ContainsKey(key))
                {
                    value2Store = this._stats[key] as int?;

                    if (value2Store.HasValue)
                    {
                        value2Store += 1;
                    }
                }

                this._stats[key] = value2Store ?? 1;
            }
        }

        public string[] GetStats()
        {
            var list = new List<string>();
            lock (this._lock)
            {
                list.AddRange(this._stats.Keys.Select(key => $"{key}: {this._stats[key]}"));
            }
            return list.ToArray();
        }

        public T GetStat<T>(string key)
        {
            T result = default(T);
            key = key.ToLower();

            lock (this._lock)
            {
                if (this._stats.ContainsKey(key))
                {
                    result = (T)this._stats[key];
                }
            }

            return result;
        }

        public void Start()
        { }

        public void Stop()
        {
            this._logger.LogInformation($"End stats: {Environment.NewLine}\t{string.Join(Environment.NewLine + "\t", this.GetStats())}");
        }
    }
}