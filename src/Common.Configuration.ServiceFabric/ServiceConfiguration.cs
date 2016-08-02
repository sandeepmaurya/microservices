using System.Fabric.Description;

namespace Common.Configuration.ServiceFabric
{
    public class ServiceConfiguration : IConfiguration
    {
        private ConfigurationSection configSection;

        public ServiceConfiguration(ConfigurationSection configSection)
        {
            this.configSection = configSection;
        }

        public string GetValue(string key)
        {
            return configSection.Parameters[key].Value;
        }
    }
}
