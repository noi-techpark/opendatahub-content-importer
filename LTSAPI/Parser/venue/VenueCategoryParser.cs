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
    public class VenueCategoryParser
    {
        public static TagLinked ParseLTSVenueCategory(
            JObject ltsresult, bool reduced
            )
        {
            string dataid = "";
            try
            {
                var ltsdata = ltsresult.ToObject<LTSData<LTSVenueCategoryData>>();
                dataid = ltsdata.data.rid;

                return ParseLTSVenueCategory(ltsdata.data, reduced);
            }
            catch(Exception ex)
            {
                //Generate Log Response on Error
                Console.WriteLine(JsonConvert.SerializeObject(new { operation = "venue.category.parse", id = dataid, source = "lts", success = false, error = true, exception = ex.Message }));

                return null;
            }          
        }

        public static TagLinked ParseLTSVenueCategory(
            LTSVenueCategoryData data, 
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

            objecttosave.MainEntity = "venue";
            objecttosave.ValidForEntity = new List<string>() { "venue" };
            objecttosave.Shortname = objecttosave.TagName.ContainsKey("en")
                ? objecttosave.TagName["en"]
                : objecttosave.TagName.FirstOrDefault().Value;
            objecttosave.Types = new List<string>() { "venuecategory" }; //TODO Clean venue categories

            //objecttosave.IDMCategoryMapping = null;
            objecttosave.PublishDataWithTagOn = null;
            objecttosave.Mapping = new Dictionary<string, IDictionary<string, string>>()
                    {
                        {
                            "lts",
                            new Dictionary<string, string>()
                            {
                                { "rid", data.rid },
                                { "code", data.code },
                                {
                                    "minimalSurfaceInSquareMeters",
                                    data.minimalSurfaceInSquareMeters.ToString()
                                },
                                { "minimalHallNumber", data.minimalHallNumber.ToString() },
                                { "isHotel", data.isHotel.ToString() },
                            }
                        },
                    };
            objecttosave.LTSTaggingInfo = null;
            objecttosave.PublishedOn = null;

            //Do not set this because we have mapped tag ids assigned
            //objecttosave.MappedTagIds = null;

            return objecttosave;
        }        
    }
}
