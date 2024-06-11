using LTSAPI;
using RabbitPusher;

namespace DataImportHelper
{
    public class DataImport
    {
        public LTSCredentials ltscredentials { get; set; }
        public LtsApi ltsapi { get; set; }

        RabbitMQSend rabbitsend { get; set; }

        public DataImport(Dictionary<string,Dictionary<string,string>> settings)
        {
            ltscredentials = new LTSCredentials() { ltsclientid = settings["lts"]["clientid"], username = settings["lts"]["username"], password = settings["lts"]["password"] };
            ltsapi = new LtsApi(ltscredentials);

            rabbitsend = new RabbitMQSend(settings["rabbitmq"]["connectionstring"]);
        }

        //This import methods are used by the Api and Console Application
        #region Import Methods

        /// <summary>
        /// Imports the LTS Amenities and Pushes it to RabbitMQ
        /// </summary>
        /// <returns></returns>
        public async Task ImportLTSAccoAmenities()
        {
            var qs = new LTSQueryStrings() { page_size = 1, filter_language = "de" };
            var dict = ltsapi.GetLTSQSDictionary(qs);

            var ltsamenities = await ltsapi.AccommodationAmenitiesRequest(null, true);
            rabbitsend.Send("lts/accommodationamenities", ltsamenities);                        
        }

        public async Task ImportLTSAccoCategories()
        {
            var qs = new LTSQueryStrings() { page_size = 1, filter_language = "de" };
            var dict = ltsapi.GetLTSQSDictionary(qs);

            var ltsamenities = await ltsapi.AccommodationCategoriesRequest(null, true);
            rabbitsend.Send("lts/accommodationcategories", ltsamenities);
        }

        public async Task ImportLTSAccoTypes()
        {
            var qs = new LTSQueryStrings() { page_size = 1, filter_language = "de" };
            var dict = ltsapi.GetLTSQSDictionary(qs);

            var ltsamenities = await ltsapi.AccommodationTypesRequest(null, true);
            rabbitsend.Send("lts/accommodationtypes", ltsamenities);
        }

        #endregion

    }
}
