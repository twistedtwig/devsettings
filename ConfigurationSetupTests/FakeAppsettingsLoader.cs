using ConfigurationSetup;

namespace ConfigurationSetupTests
{
    public class FakeAppsettingsLoader : IAppSettingsLoader
    {
        private readonly string _key;
        private readonly string _value;

        public FakeAppsettingsLoader(string key, string value)
        {
            _key = key;
            _value = value;
        }

        public bool ContainsKey(string key)
        {
            return _key == key;
        }

        public string Get(string key)
        {
            return _key == key ? _value : string.Empty;
        }
    }
}
