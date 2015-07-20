using CustomConfigurations;
using System;
using System.Linq;

namespace ConfigurationSetup
{
    public class ConfigurationLoader<T>
    {
        private readonly Config _config;
        private readonly ConfigSection _globalSection;
        private readonly OverrideType _overrideType;

        private readonly IMachineNameFinder _machineNameFinder;
        private readonly IAppSettingsLoader _appSettings;

        public ConfigurationLoader(string globalSettingsName = "global", OverrideType overrideType = OverrideType.Chain) :this(new MachineNameFinder(), new AppSettingsLoader(), globalSettingsName, overrideType)
        { }

        /// <summary>
        /// Create the configuration loader
        /// </summary>
        /// <param name="machineNameFinder">abstracts out the accessing of what the machine name is</param>
        /// <param name="appSettings">abstacts out the process of accessing app settings</param>
        /// <param name="globalSettingsName">the name of the section within the configSectionName that will act as the base global settings.  This is defaulted to global</param>
        /// <param name="overrideType">The type of override wanted.  By default it will use chaining, which will try to find first valid type. Env - Appsetting - Machine name.</param>
        public ConfigurationLoader(IMachineNameFinder machineNameFinder, IAppSettingsLoader appSettings, string globalSettingsName = "global", OverrideType overrideType = OverrideType.Chain)
        {
            _config =  new Config();            
            _overrideType = overrideType;

            if (!_config.SectionNames.Contains(globalSettingsName))
            {
                throw new ArgumentOutOfRangeException("globalSettingsName", "No global section found");                
            }

            _globalSection = _config.GetSection(globalSettingsName);
            _machineNameFinder = machineNameFinder;
            _appSettings = appSettings;
        }

        /// <summary>
        /// Creates the strongly typed settings file with the given overrides if found.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Create(string key = "")
        {
            var globalModel = _globalSection.Create<T>();

            switch (_overrideType)
            {
                case OverrideType.Chain:

                    if (PopulateFromEnvironmentVariables(globalModel, key))
                    {
                        return globalModel;
                    }

                    if (PopulateFromAppSetting(globalModel, key))
                    {
                        return globalModel;
                    }

                    if (PopulateFromMachineName(globalModel))
                    {
                        return globalModel;
                    }

                    return globalModel;

                case OverrideType.EnvironmentalVariable:
                    PopulateFromEnvironmentVariables(globalModel, key);
                    return globalModel;

                case OverrideType.MachineName:
                    PopulateFromMachineName(globalModel);
                    return globalModel;

                case OverrideType.AppSettingKey:
                    PopulateFromAppSetting(globalModel, key);
                    return globalModel;
                    
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool PopulateFromEnvironmentVariables(T globalModel, string key)
        {
            var sectionName = Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                return false;
            }

            return PopulateModel(globalModel, sectionName);
        }

        private bool PopulateFromMachineName(T globalModel)
        {
            var machineName = _machineNameFinder.GetMachineName();

            if (!_config.SectionNames.Contains(machineName))
            {
                return false;
            }

            return PopulateModel(globalModel, machineName);
        }

        private bool PopulateFromAppSetting(T globalModel, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return false; //no key provided
            }

            if (!_appSettings.ContainsKey(key))
            {
                return false; //no key in app settings
            }

            var appsetting = _appSettings.Get(key);
            return PopulateModel(globalModel, appsetting);
        }

        private bool PopulateModel(T globalModel, string sectionName)
        {
            var subSection = _config.GetSection(sectionName);
            if (subSection == null)
            {
                return false;
            }

            subSection.Populate(globalModel);

            return true;
        }
    }
}
