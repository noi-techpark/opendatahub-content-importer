using GenericHelper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericHelper
{
    public class Settings : ISettings
    {
        private readonly IConfiguration configuration;
        private readonly Lazy<string> rabbitConnection;        
        private readonly Lazy<string> mongoDBConnection;
        private readonly LTSCredentials ltsCredentials;
        private readonly LTSCredentials ltsCredentialsOpen;
        private readonly HGVCredentials hgvCredentials;
        private readonly RabbitMQConfiguration rabbitMQConfiguration;
        private readonly ODHApiCoreConfig odhApiCoreConfiguration;

        public Settings(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.rabbitConnection = new Lazy<string>(() =>
            this.configuration.GetConnectionString("RabbitConnection"));
            this.mongoDBConnection = new Lazy<string>(() =>
            this.configuration.GetConnectionString("MongoDBConnection"));

        
            var ltsapi = this.configuration.GetSection("LTSApiIDM");
            this.ltsCredentials = new LTSCredentials(ltsapi.GetValue<string>("serviceurl", ""), ltsapi.GetValue<string>("username", ""), ltsapi.GetValue<string>("password", ""), ltsapi.GetValue<string>("xltsclientid", ""), ltsapi.GetValue<bool>("opendata", false));

            var ltsapiopen = this.configuration.GetSection("LTSApiNOI");
            this.ltsCredentialsOpen = new LTSCredentials(ltsapiopen.GetValue<string>("serviceurl", ""), ltsapiopen.GetValue<string>("username", ""), ltsapiopen.GetValue<string>("password", ""), ltsapiopen.GetValue<string>("xltsclientid", ""), ltsapiopen.GetValue<bool>("opendata", false));

            var hgvapi = this.configuration.GetSection("HGVApi");
            this.hgvCredentials = new HGVCredentials(hgvapi.GetValue<string>("serviceurl", ""), hgvapi.GetValue<string>("username", ""), hgvapi.GetValue<string>("password", ""));


            var rabbitMQconfigsection = this.configuration.GetSection("RabbitMQConfiguration");
            this.rabbitMQConfiguration = new RabbitMQConfiguration() { ReadQueue = rabbitMQconfigsection.GetValue<string>("ReadQueue", "") };


            var odhapicoresection = this.configuration.GetSection("ODHApiCore");
            this.odhApiCoreConfiguration = new ODHApiCoreConfig() {
                ClientId = odhapicoresection.GetValue<string>("client_id", ""),
                ClientSecret = odhapicoresection.GetValue<string>("client_secret", ""),
                AuthServer = odhapicoresection.GetValue<string>("authserver", ""),
                ApiEndpoint = odhapicoresection.GetValue<string>("apiurl", ""),
            };
        }

        public string RabbitConnection => this.rabbitConnection.Value;
        public string MongoDBConnection => this.mongoDBConnection.Value;

        public LTSCredentials LtsCredentials => this.ltsCredentials;

        public LTSCredentials LtsCredentialsOpen => this.ltsCredentialsOpen;

        public HGVCredentials HgvCredentials => this.hgvCredentials;

        public RabbitMQConfiguration RabbitMQConfiguration => this.rabbitMQConfiguration;

        public ODHApiCoreConfig ODHApiCoreConfiguration => this.odhApiCoreConfiguration;
    }
}
