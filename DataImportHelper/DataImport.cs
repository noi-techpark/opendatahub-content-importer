using LTSAPI;
using RabbitPusher;

namespace DataImportHelper
{
    public class DataImport
    {
        public LTSCredentials ltscredentials { get; set; }
        public LtsApi ltsapi { get; set; }

        public string opendata { get; set; }

        RabbitMQSend rabbitsend { get; set; }

        public DataImport(Dictionary<string,Dictionary<string,string>> settings, bool open = false)
        {
            ltscredentials = new LTSCredentials() { ltsclientid = settings["lts"]["clientid"], username = settings["lts"]["username"], password = settings["lts"]["password"] };
            ltsapi = new LtsApi(ltscredentials);
            if (open)
                opendata = "_opendata";
            else
                opendata = "";

            rabbitsend = new RabbitMQSend(settings["rabbitmq"]["connectionstring"]);
        }

        //This import methods are used by the Api and Console Application
        #region Import Methods

        /// <summary>
        /// Imports the LTS Accommodation Amenities and pushes it to RabbitMQ
        /// </summary>
        /// <returns></returns>
        public async Task ImportLTSAccoAmenities()
        {
            var qs = new LTSQueryStrings() { page_size = 1, filter_language = "de" };
            var dict = ltsapi.GetLTSQSDictionary(qs);

            var ltsdata = await ltsapi.AccommodationAmenitiesRequest(null, true);
            rabbitsend.Send("lts/accommodationamenities", ltsdata);                        
        }

        /// <summary>
        /// Imports the LTS Accommodation Categories and pushes it to RabbitMQ
        /// </summary>
        /// <returns></returns>
        public async Task ImportLTSAccoCategories()
        {
            var qs = new LTSQueryStrings() { page_size = 1, filter_language = "de" };
            var dict = ltsapi.GetLTSQSDictionary(qs);

            var ltsdata = await ltsapi.AccommodationCategoriesRequest(null, true);
            rabbitsend.Send("lts/accommodationcategories", ltsdata);
        }

        /// <summary>
        ///  Imports the LTS Accommodation Types and pushes it to RabbitMQ
        /// </summary>
        /// <returns></returns>
        public async Task ImportLTSAccoTypes()
        {
            var qs = new LTSQueryStrings() { page_size = 1, filter_language = "de" };
            var dict = ltsapi.GetLTSQSDictionary(qs);

            var ltsdata = await ltsapi.AccommodationTypesRequest(null, true);
            rabbitsend.Send("lts/accommodationtypes", ltsdata);
        }

        public async Task ImportLTSAccommodationChanged(DateTime datefrom)
        {
            var qs = new LTSQueryStrings() { 
                page_size = 100, 
                filter_lastUpdate = datefrom,
                filter_marketingGroupRids = "9E72B78AC5B14A9DB6BED6C2592483BF",
                fields = "rid" //when fields rid is set lts api gives all pages without paging
            };
            var dict = ltsapi.GetLTSQSDictionary(qs);

            var ltsdata = await ltsapi.AccommodationListRequest(dict, true);
            rabbitsend.Send("lts/accommodationchanged", ltsdata);
        }

        public async Task ImportLTSAccommodationDeleted(DateTime datefrom)
        {
            var qs = new LTSQueryStrings() { page_size = 1, filter_lastUpdate = datefrom };
            var dict = ltsapi.GetLTSQSDictionary(qs);

            var ltsdata = await ltsapi.AccommodationDeleteRequest(dict, true);
            rabbitsend.Send("lts/accommodationdeleted", ltsdata);
        }

        public async Task ImportLTSAccommodationSingle(string rid)
        {
            var qs = new LTSQueryStrings() { page_size = 1 };
            var dict = ltsapi.GetLTSQSDictionary(qs);

            var ltsdata = await ltsapi.AccommodationDetailRequest(rid, null);
            rabbitsend.Send("lts/accommodationdetail" + opendata, ltsdata);
        }

        #endregion

    }
}
