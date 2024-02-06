using Microsoft.Extensions.Configuration;

namespace LDAP.Authuntication
{
    public interface ILDAPConfig
    {
        string Server { get; }
        int Port { get; }
        string Domain { get; }
        string Subdomain { get; }
        string Zone { get; }
        string[] Groups { get; }
    }

    public class LDAPConfig : ILDAPConfig
    {
        private readonly IConfiguration _configuration;

        public LDAPConfig()
        {
            _configuration = //configuration;
                new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("LDAPconfig.json") //LDAPconfig.json
                .Build(); // Set base path to the current domain directory
        }

        public string Server => _configuration["LDAPServer:Server"]!;

        public int Port => Convert.ToInt16(_configuration["LDAPServer:Port"]);

        public string Domain => _configuration["LDAPServer:Domain"]!;

        public string Subdomain => _configuration["LDAPServer:Subdomain"]!;

        public string Zone => _configuration["LDAPServer:Zone"]!;

        public string[] Groups => GetGroups();

        public string[] GetGroups()
        {
            IConfigurationSection groupsSection = _configuration.GetSection("LDAPServer:Groups");
            // Use GetChildren() to iterate through the array elements
            IEnumerable<IConfigurationSection> groupSections = groupsSection.GetChildren();

            // Extract values from children sections
            string[] groups = groupSections.Select(x => x.Value).ToArray()!;

            return groups;
        }
    }
}
