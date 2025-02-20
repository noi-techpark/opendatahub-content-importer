using DataModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;
using GenericHelper;
using Newtonsoft.Json;

namespace LTSAPI.Parser
{
    public class GastronomyFacilitiesParser
    {
        public static TagLinked ParseLTSGastronomyFacilities(
            JObject ltsresult, bool reduced
            )
        {
            string dataid = "";
            try
            {
                var ltsdata = ltsresult.ToObject<LTSData<LTSGastronomyFacilityData>>();
                dataid = ltsdata.data.rid;

                return ParseLTSGastronomyFacilities(ltsdata.data, reduced);
            }
            catch(Exception ex)
            {
                //Generate Log Response on Error
                Console.WriteLine(JsonConvert.SerializeObject(new { operation = "gastronomy.facility.parse", id = dataid, source = "lts", success = false, error = true, exception = ex.Message }));

                return null;
            }          
        }

        public static TagLinked ParseLTSGastronomyFacilities(
            LTSGastronomyFacilityData data, 
            bool reduced)
        {        
            TagLinked objecttosave = new TagLinked();

            objecttosave.Id = data.rid;
            objecttosave.Active = true;
            objecttosave.DisplayAsCategory = false;
            objecttosave.FirstImport =
                objecttosave.FirstImport == null ? DateTime.Now : objecttosave.FirstImport;
            objecttosave.LastChange = data.lastUpdate;

            objecttosave.Source = "lts";
            objecttosave.TagName = data.name;
            objecttosave.Description = data.description;

            objecttosave.MainEntity = "odhactivitypoi";
            objecttosave.ValidForEntity = new List<string>()
                    {
                        "odhactivitypoi",
                        "gastronomy",
                    };
            objecttosave.Shortname = objecttosave.TagName.ContainsKey("en")
                ? objecttosave.TagName["en"]
                : objecttosave.TagName.FirstOrDefault().Value;

            string matchedtype = GetTypeFromFacilityGroup(data.group);

            objecttosave.Types = new List<string>() { matchedtype, "gastronomyfacilities" };

            objecttosave.IDMCategoryMapping = null;
            objecttosave.PublishDataWithTagOn = null;
            objecttosave.Mapping = new Dictionary<string, IDictionary<string, string>>()
                    {
                        {
                            "lts",
                            new Dictionary<string, string>()
                            {
                                { "rid", data.rid },
                                { "code", data.code },
                                { "group", data.group },
                            }
                        },
                    };
            objecttosave.LTSTaggingInfo = null;
            objecttosave.PublishedOn = null;

            //Do not set this because we have mapped tag ids assigned
            objecttosave.MappedTagIds = null;

            return objecttosave;
        }

        private static string GetTypeFromFacilityGroup(string group)
        {
            return group switch
            {
                "RS_Cuisine" => "facilitycodes_cuisinecodes",
                "RS_Equipment" => "facilitycodes_equipment",
                "RS_QualitySeals" => "facilitycodes_qualityseals",
                "RS_CreditCard" => "facilitycodes_creditcard",
                _ => "gastronomyfacilities",
            };
        }
    }
}
