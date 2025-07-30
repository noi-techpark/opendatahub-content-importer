﻿// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
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
    public class TagParser
    {
        public static TagLinked ParseLTSTag(
            JObject taglts, bool reduced
            )
        {
            string dataid = "";
            try
            {
                LTSTags ltstag = taglts.ToObject<LTSTags>();

                dataid = ltstag.data.rid;

                return ParseLTSTag(ltstag.data, reduced);
            }
            catch(Exception ex)
            {
                //Generate Log Response on Error
                Console.WriteLine(JsonConvert.SerializeObject(new { operation = "tag.parse", id = dataid, source = "lts", success = false, error = true, exception = ex.Message }));

                return null;
            }          
        }

        public static TagLinked ParseLTSTag(
            LTSTagsData ltstag, 
            bool reduced)
        {
            TagLinked tag = new TagLinked();

            tag.Id = ltstag.rid;
            tag._Meta = new Metadata() { Id = tag.Id, LastUpdate = DateTime.Now, Reduced = reduced, Source = "lts", Type = "tag", UpdateInfo = new UpdateInfo() { UpdatedBy = "importer.v2", UpdateSource = "lts.interface.v2" } };
            tag.Source = "lts";

            tag.LastChange = ltstag.lastUpdate;

            tag.Active = ltstag.isActive;
            tag.DisplayAsCategory = false;
            tag.FirstImport =
                tag.FirstImport == null ? DateTime.Now : tag.FirstImport;            
            
            tag.TagName = ltstag.name;
            tag.Description = ltstag.description;

            tag.MainEntity = "odhactivitypoi";
            tag.ValidForEntity = new List<string>() { "odhactivitypoi" };
            tag.Shortname = tag.TagName.ContainsKey("en")
                ? tag.TagName["en"]
                : tag.TagName.FirstOrDefault().Value;
            tag.Types = new List<string>() { "tags" + ltstag.entityType.ToLower() };
            
            tag.PublishDataWithTagOn = null;
            tag.Mapping = new Dictionary<string, IDictionary<string, string>>()
                    {
                        {
                            "lts",
                            new Dictionary<string, string>()
                            {
                                { "rid", ltstag.rid },
                                { "code", ltstag.code },
                                { "entityType", ltstag.entityType },
                                { "level", ltstag.level.ToString() },
                                { "mainTagRid", ltstag.mainTagRid },
                                { "parentTagRid", ltstag.parentTagRid },
                                { "isSelectable", ltstag.isSelectable.ToString() },
                            }
                        },
                    };
            tag.LTSTaggingInfo = new LTSTaggingInfo()
            {
                LTSRID = ltstag.rid,
                ParentLTSRID = ltstag.parentTagRid,
            };
            tag.PublishedOn = null;


            return tag;
        }
    }

}
