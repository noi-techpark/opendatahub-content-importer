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
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;
using GenericHelper;
using Newtonsoft.Json;

namespace LTSAPI.Parser
{
    public class EventCategoriesParser
    {
        public static TagLinked ParseLTSEventCategory(
            JObject ltsresult, bool reduced
            )
        {
            string dataid = "";
            try
            {
                var ltsdata = ltsresult.ToObject<LTSData<LTSEventCategoryData>>();
                dataid = ltsdata.data.rid;

                return ParseLTSEventCategory(ltsdata.data, reduced);
            }
            catch(Exception ex)
            {
                //Generate Log Response on Error
                Console.WriteLine(JsonConvert.SerializeObject(new { operation = "event.category.parse", id = dataid, source = "lts", success = false, error = true, exception = ex.Message }));

                return null;
            }          
        }

        public static TagLinked ParseLTSEventCategory(
            LTSEventCategoryData data, 
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

            objecttosave.MainEntity = "event";
            objecttosave.ValidForEntity = new List<string>() { "event" };
            objecttosave.Shortname = objecttosave.TagName.ContainsKey("en")
                ? objecttosave.TagName["en"]
                : objecttosave.TagName.FirstOrDefault().Value;
            objecttosave.Types = new List<string>() { "eventcategory", "eventtopic" };

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
                                { "classificationrid", data.classification.rid },
                            }
                        },
                    };
            objecttosave.LTSTaggingInfo = null;
            objecttosave.PublishedOn = null;
            objecttosave.MappedTagIds = null;

            return objecttosave;
        }
    }
}
