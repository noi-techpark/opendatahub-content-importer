using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public interface ISettings
    {
        string RabbitConnection { get; }
        string MongoDBConnection { get; }

        LTSCredentials LtsCredentials { get; }

        LTSCredentials LtsCredentialsOpen { get; }

        RabbitMQConfiguration RabbitMQConfiguration { get; }

        ODHApiCoreConfig ODHApiCoreConfiguration { get; }
    }

    public class LTSCredentials
    {
        public LTSCredentials(string username, string password, string ltsclientid, bool opendata)
        {
            this.ltsclientid = ltsclientid;
            this.username = username;
            this.password = password;
            this.opendata = opendata;
        }

        public string ltsclientid { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public bool opendata { get; set; }
    }

    public class RabbitMQConfiguration
    {
        public string? ReadQueue { get; set; }
    }

    public class ODHApiCoreConfig
    {
        public string? AuthServer { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? ApiEndpoint { get; set; }
    }
}
