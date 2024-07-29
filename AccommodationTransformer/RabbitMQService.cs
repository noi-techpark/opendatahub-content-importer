// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using AccommodationTransformer.Parser;
using DataImportHelper;
using Helper;
using Microsoft.Extensions.Configuration;
using MongoDBConnector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitPusher;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using TransformerHelper;

namespace AccommodationTransformer
{   
    public interface IReadAccommodation : IReadMessage
    {

    }

    public class ReadAccommodation : ReadMessage, IReadAccommodation
    {
        public override async Task<bool> TransformData(MongoDBObject data, string routingkey)
        {
        
            var json = JToken.Parse(data.rawdata);
            var jsonarray = JArray.FromObject(json);

            if (routingkey == "lts.accommodationchanged")
            {
                Console.WriteLine("Read Accommodation CHANGED called");

                var jsonarraydata = jsonarray.FirstOrDefault()["data"].Value<JArray>();

                foreach (var ridobj in jsonarraydata)
                {
                    var rid = ridobj["rid"].Value<string>();

                    Console.WriteLine("rid: " + rid);

                    await dataimport["idm"].ImportLTSAccommodationSingle(rid);

                    await dataimport["open"].ImportLTSAccommodationSingle(rid);
                }

                //Request 

                return true;
            }
            else if (routingkey == "lts.accommodationdetail")
            {
                Console.WriteLine("Read Accommodation DETAIL called");
               
                JObject accomodationdetail = jsonarray.FirstOrDefault().Value<JObject>();

                //TODO PARSE ACCOMMODATION
                var name = accomodationdetail["data"]["contacts"].Value<JArray>().FirstOrDefault()["address"]["name"].Value<JObject>();
                              
                Console.WriteLine("Processing Accommodation " + accomodationdetail["data"]["rid"].Value<string>());

                //Load all XDocuments
                var xmlfiles = LoadXmlFiles("../xml/");

                var result = AccommodationParser.ParseLTSAccommodation(accomodationdetail, false, xmlfiles);

                //Write to the ODH Api and pass referer + use a service account
                var apiresponse = await writetoodhapi.PushToODHApiCore(result, result.Id, "Accommodation");

                return true;
            }
            else if (routingkey == "lts.accommodationdetail_open")
            {
                Console.WriteLine("Read Accommodation DETAIL OPEN called");

                JObject accomodationdetail = jsonarray.FirstOrDefault()["data"].Value<JObject>();

                //TODO PARSE ACCOMMODATION
                var name = accomodationdetail["contacts"].Value<JArray>().FirstOrDefault()["address"]["name"].Value<JObject>();
                string namede = "";

                if (name != null)
                {
                    JToken token = name["de"];
                    if (token != null)
                    {
                        namede = token.Value<string>();
                    }
                }

                Console.WriteLine("Processing Accommodation " + accomodationdetail["rid"].Value<string>() + " " + namede);

                return true;
            }
            else
                return false;

            //Push all Rids to accommodation/detail
            //RabbitMQSend rabbitsend = new RabbitMQSend(rabbitmqconnection);
            //var ltsamenities = await ltsapi.AccommodationAmenitiesRequest(null, true);
            //rabbitsend.Send("lts/accommodationamenities", ltsamenities);
        }

        public IDictionary<string, XDocument> LoadXmlFiles(string directory)
        {
            //TODO move this files to Database

            IDictionary<string, XDocument> myxmlfiles = new Dictionary<string, XDocument>();
            myxmlfiles.Add("AccoCategories", XDocument.Load(directory + "AccoCategories.xml"));
            myxmlfiles.Add("AccoTypes", XDocument.Load(directory + "AccoTypes.xml"));
            myxmlfiles.Add("Alpine", XDocument.Load(directory + "Alpine.xml"));
            myxmlfiles.Add("Boards", XDocument.Load(directory + "Boards.xml"));
            myxmlfiles.Add("City", XDocument.Load(directory + "City.xml"));
            myxmlfiles.Add("Dolomites", XDocument.Load(directory + "Dolomites.xml"));
            myxmlfiles.Add("Features", XDocument.Load(directory + "Features.xml"));
            myxmlfiles.Add("Mediterranean", XDocument.Load(directory + "Mediterranean.xml"));
            myxmlfiles.Add("NearSkiArea", XDocument.Load(directory + "NearSkiArea.xml"));
            myxmlfiles.Add("RoomAmenities", XDocument.Load(directory + "RoomAmenities.xml"));
            myxmlfiles.Add("Vinum", XDocument.Load(directory + "Vinum.xml"));
            myxmlfiles.Add("Wine", XDocument.Load(directory + "Wine.xml"));

            return myxmlfiles;
        }
    }    

}
