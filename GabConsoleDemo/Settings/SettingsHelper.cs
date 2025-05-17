using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GabConsoleDemo.Settings
{
    internal class SettingsHelper<T> : ISettings<T> where T : struct
    {
        private static SettingsHelper<T> _instance = null;
        private static readonly object _lock = new object();
        public T _settings;
        public static SettingsHelper<T> Instance
        { 
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        string configFileName = $"{AppContext.BaseDirectory}Settings\\SettingsConfig\\{typeof(T).Name}.json";
                        string configTemplateFileName = $"{AppContext.BaseDirectory}Settings\\SettingsTemplates\\{typeof(T).Name}.template.json";

                        if (!File.Exists(configFileName))
                        {
                            throw new FileNotFoundException($"The file {configFileName} does not exist.");
                        }
                        if (!File.Exists(configTemplateFileName))
                        {
                            throw new FileNotFoundException($"The file {configTemplateFileName} does not exist.");
                        }
                        if (ValidateConfig(configFileName, configTemplateFileName))
                        {
                            var configJson = File.ReadAllText(configFileName);
                            _instance = new SettingsHelper<T>();
                            _instance._settings = JsonConvert.DeserializeObject<T>(configJson);
                        }
                    }
                }
                return _instance;
            }
        }
        public static bool ValidateConfig(string jsonFilePath, string jsonConfigTemplatePath)
        {
            // Load the JSON file and template
            var configJson = File.ReadAllText(jsonFilePath);
            var templateJson = File.ReadAllText(jsonConfigTemplatePath);
            // Validate the JSON file against the template

            var templateSettings = (JsonConvert.DeserializeObject<JObject>(templateJson) as IDictionary<string, JToken?>).Keys.ToList();
            var configSettings = (JsonConvert.DeserializeObject<JObject>(configJson) as IDictionary<string, JToken?>).Keys.ToList();

            if (templateSettings.Count != configSettings.Count)
            {
                throw new Exception("Config keys mismatch. Fix config template");
            }

            foreach (var item in templateSettings)
            {
                if (!configSettings.Contains(item))
                {
                    throw new Exception($"Key {item} is missing from your configuration");
                }
            }
            return true;
        }
    }
}
