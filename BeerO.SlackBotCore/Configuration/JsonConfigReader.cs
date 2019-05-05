using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace BeerO.SlackBotCore.Configuration
{
    /// <summary>
    /// Use static helper methods to construct
    /// </summary>
    public class JsonConfigReader : IConfigReader
    {
        private JObject _currentJObject;
        private readonly object _lock = new object();
        private readonly string _absoluteFileName;
        private const string SLACKAPI_CONFIGVALUE = "slack:apiToken";

        private JsonConfigReader(string absoluteFileName)
        {
            this._absoluteFileName = absoluteFileName;
        }

        public bool HelpEnabled { get; set; } = true;
        public bool StatsEnabled { get; set; } = true;
        public bool AboutEnabled { get; set; } = true;

        public string SlackApiKey => this.GetConfigEntry<string>(SLACKAPI_CONFIGVALUE);

        public T GetConfigEntry<T>(string entryName)
        {
            return this.GetJObject().Value<T>(entryName);
        }

        private JObject GetJObject()
        {
            lock (this._lock)
            {
                if (this._currentJObject == null)
                {
                    string json = File.ReadAllText(this._absoluteFileName);
                    this._currentJObject = JObject.Parse(json);
                }
            }

            return this._currentJObject;
        }

        public static JsonConfigReader DefaultLocation()
        {
            return ForRelativePath();
        }

        public static JsonConfigReader ForAbsolutePath(string absolutePath)
        {
            return new JsonConfigReader(absolutePath);
        }

        public static JsonConfigReader ForRelativePath(string customRelativePath = null)
        {
            var path = string.IsNullOrEmpty(customRelativePath)
                ? Path.Combine("configuration", "config.json")
                : customRelativePath;

            var fullPath = Path.Combine(AssemblyLocation(), path);
            return new JsonConfigReader(fullPath);
        }

        private static string AssemblyLocation()
        {
            var assembly = typeof(JsonConfigReader).GetTypeInfo().Assembly;
            var codebase = new Uri(assembly.CodeBase);
            var path = Path.GetDirectoryName(codebase.LocalPath);
            return path;
        }
    }
}