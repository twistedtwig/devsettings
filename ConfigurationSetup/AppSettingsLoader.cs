using System.Configuration;
using System.Linq;

namespace ConfigurationSetup
{
    public interface IAppSettingsLoader
    {
        bool ContainsKey(string key);
        string Get(string key);
    }

    public class AppSettingsLoader : IAppSettingsLoader
    {
        public bool ContainsKey(string key)
        {
            return ConfigurationManager.AppSettings.AllKeys.Contains(key);
        }

        public string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
