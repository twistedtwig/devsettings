using System;

namespace ConfigurationSetup
{
    public interface IMachineNameFinder
    {
        string GetMachineName();
    }

    public class MachineNameFinder : IMachineNameFinder
    {
        public string GetMachineName()
        {
            return Environment.MachineName;
        }
    }
}
