using ConfigurationSetup;

namespace ConfigurationSetupTests
{
    public class FakeMachineNameFinder : IMachineNameFinder
    {
        private readonly string _name;

        public FakeMachineNameFinder(string name)
        {
            this._name = name;
        }

        public string GetMachineName()
        {
            return _name;
        }
    }
}
