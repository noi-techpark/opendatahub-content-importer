// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

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
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json;

namespace LTSAPI.Parser
{
    public class AccommodationAmenitiesParser
    {
        public static TagLinked ParseLTSAccommodationAmenities(
            JObject ltsresult, bool reduced
            )
        {
            string dataid = "";
            try
            {
                var ltsdata = ltsresult.ToObject<LTSData<LTSAccommodationAmenityData>>();
                dataid = ltsdata.data.rid;

                return ParseLTSAccommodationAmenities(ltsdata.data, reduced);
            }
            catch(Exception ex)
            {
                //Generate Log Response on Error
                Console.WriteLine(JsonConvert.SerializeObject(new { operation = "accommodation.amenity.parse", id = dataid, source = "lts", success = false, error = true, exception = ex.Message }));

                return null;
            }          
        }

        public static TagLinked ParseLTSAccommodationAmenities(
            LTSAccommodationAmenityData data, 
            bool reduced)
        {
            List<string> typelistlts = new List<string>();

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

            objecttosave.MainEntity = "accommodation";

            if (objecttosave.ValidForEntity == null)
                objecttosave.ValidForEntity = new List<string>() { "accommodation" };
            else
            {
                if (!objecttosave.ValidForEntity.Contains("accommodation"))
                    objecttosave.ValidForEntity.Add("accommodation");
            }

            objecttosave.Shortname = objecttosave.TagName.ContainsKey("en")
                ? objecttosave.TagName["en"]
                : objecttosave.TagName.FirstOrDefault().Value;

            if (objecttosave.Types == null)
                objecttosave.Types = new List<string>() { "accommodation" + data.type };
            else
            {
                if (!objecttosave.Types.Contains("accommodation" + data.type))
                    objecttosave.Types.Add("accommodation" + data.type);
            }

            if (!typelistlts.Contains("accommodation" + data.type))
                typelistlts.Add("accommodation" + data.type);

            //objecttosave.IDMCategoryMapping = null;
            objecttosave.PublishDataWithTagOn = null;
            if (objecttosave.Mapping == null || !objecttosave.Mapping.ContainsKey("lts"))
                objecttosave.Mapping = new Dictionary<string, IDictionary<string, string>>()
                        {
                            {
                                "lts",
                                new Dictionary<string, string>()
                                {
                                    { "rid", data.rid },
                                    { "code", data.code },
                                    { "type", data.type },
                                }
                            },
                        };
            else
            {
                objecttosave.Mapping["lts"].TryAddOrUpdate("rid", data.rid);
                objecttosave.Mapping["lts"].TryAddOrUpdate("code", data.code);
                objecttosave.Mapping["lts"].TryAddOrUpdate("type", data.type);
            }

            objecttosave.LTSTaggingInfo = null;
            objecttosave.PublishedOn = null;

            return objecttosave;
        }
    }
}
