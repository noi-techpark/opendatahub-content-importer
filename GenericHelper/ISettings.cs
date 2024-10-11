using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericHelper
{
    public interface ISettings
    {
        string RabbitConnection { get; }
        string MongoDBConnection { get; }

        LTSCredentials LtsCredentials { get; }

        LTSCredentials LtsCredentialsOpen { get; }

        HGVCredentials HgvCredentials{ get; }

        RabbitMQConfiguration RabbitMQConfiguration { get; }

        ODHApiCoreConfig ODHApiCoreConfiguration { get; }
    }

    public class LTSCredentials
    {
        public LTSCredentials(string serviceurl, string username, string password, string ltsclientid, bool opendata)
        {
            this.serviceurl = serviceurl;
            this.ltsclientid = ltsclientid;
            this.username = username;
            this.password = password;
            this.opendata = opendata;
        }

        public string serviceurl { get; set; }
        public string ltsclientid { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public bool opendata { get; set; }
    }

    public class HGVCredentials
    {
        public HGVCredentials(string serviceurl, string username, string password)
        {
            this.serviceurl = serviceurl;
            this.username = username;
            this.password = password;            
        }

        public string serviceurl { get; set; }
        public string username { get; set; }
        public string password { get; set; }        
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
