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
    public class GuestPassCardTypesParser
    {
        public static TagLinked ParseLTSGuestPassCardTypes(
            JObject ltsresult, bool reduced
            )
        {
            string dataid = "";
            try
            {
                var ltsdata = ltsresult.ToObject<LTSData<LTSGuestcardData>>();
                dataid = ltsdata.data.rid;

                return ParseLTSGuestPassCardTypes(ltsdata.data, reduced);
            }
            catch(Exception ex)
            {
                //Generate Log Response on Error
                Console.WriteLine(JsonConvert.SerializeObject(new { operation = "suedtirolguestpass.cardtype.parse", id = dataid, source = "lts", success = false, error = true, exception = ex.Message }));

                return null;
            }          
        }

        public static TagLinked ParseLTSGuestPassCardTypes(
            LTSGuestcardData data, 
            bool reduced)
        {        
            TagLinked objecttosave = new TagLinked();

            objecttosave.Id = data.rid;
            objecttosave.Active = data.isActive;
            objecttosave.DisplayAsCategory = false;
            objecttosave.FirstImport =
                objecttosave.FirstImport == null ? DateTime.Now : objecttosave.FirstImport;
            objecttosave.LastChange = data.lastUpdate;

            objecttosave.Source = data.rid == "guestcard" ? "idm" : "lts";
            objecttosave.TagName = data.name;
            objecttosave.MainEntity = "accommodation";
            objecttosave.ValidForEntity = new List<string>() { "accommodation" };
            objecttosave.Shortname = objecttosave.TagName.ContainsKey("en")
                ? objecttosave.TagName["en"]
                : objecttosave.TagName.FirstOrDefault().Value;
            objecttosave.Types = new List<string>() { "cardtype" };

            objecttosave.IDMCategoryMapping = null;
            objecttosave.PublishDataWithTagOn = null;
            objecttosave.Mapping = new Dictionary<string, IDictionary<string, string>>()
                    {
                        {
                            "lts",
                            new Dictionary<string, string>() { { "rid", data.rid } }
                        },
                    };
            ;
            objecttosave.LTSTaggingInfo = null;
            objecttosave.PublishedOn = null;
            objecttosave.MappedTagIds = null;

            return objecttosave;
        }       
    }
}
