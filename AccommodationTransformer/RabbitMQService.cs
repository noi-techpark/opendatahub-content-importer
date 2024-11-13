// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataImportHelper;
using DataModel;
using GenericHelper;
using LTSAPI;
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
using System.Security.Cryptography;
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

                    await dataimport["idm"].ImportLTSAccommodationSingle(rid, rid);

                    await dataimport["open"].ImportLTSAccommodationSingle(rid);
                }

                //Request 

                return true;
            }
            else if (routingkey == "lts.accommodationdetail")
            {
                Console.WriteLine("Read Accommodation DETAIL called");

                JObject accomodationdetail = jsonarray.FirstOrDefault().Value<JObject>();

                ////TEST PArsing
                //var test = accomodationdetail["data"].ToObject<LTSAccoData>();

                var identifier = accomodationdetail["data"]["rid"].Value<string>();
                            
                //GET HGV Room Data if 
                if (!String.IsNullOrEmpty(accomodationdetail["data"]["hgvid"].Value<string>()))
                    await dataimport["idm"].ImportHGVAccommodationRoomList(identifier, data.id);
                else
                {                    
                    await dataimport["idm"].ImportAccommodationFinished(JObject.FromObject(new AccommodationFinished() { Id = identifier, HasHGVRooms = false }), data.id);
                }

                return true;
            }
            //else if (routingkey == "lts.accommodationdetail_open")
            //{
            //    Console.WriteLine("Read Accommodation DETAIL OPEN called");

            //    JObject accomodationdetail = jsonarray.FirstOrDefault()["data"].Value<JObject>();

            //    //TODO PARSE ACCOMMODATION
            //    var name = accomodationdetail["contacts"].Value<JArray>().FirstOrDefault()["address"]["name"].Value<JObject>();
            //    string namede = "";

            //    if (name != null)
            //    {
            //        JToken token = name["de"];
            //        if (token != null)
            //        {
            //            namede = token.Value<string>();
            //        }
            //    }

            //    Console.WriteLine("Processing Accommodation " + accomodationdetail["rid"].Value<string>() + " " + namede);

            //    return true;
            //}
            else if (routingkey == "hgv.accoommodationroom")
            {
                //Launch accommodation Room HGV Import

                Console.WriteLine("Read Accommodation HGV ROOMS called");
           
                await dataimport["idm"].ImportAccommodationFinished(JObject.FromObject(new AccommodationFinished() { Id = data.id, HasHGVRooms = true }), data.id);

                return true;
            }
            else if (routingkey == "base.accommodationimported")
            {
                //If this is called all Information was collected

                JObject accommodationimporteddetailraw = jsonarray.FirstOrDefault().Value<JObject>();
                var accommodationimporteddetail = accommodationimporteddetailraw.ToObject<AccommodationFinished>();

                var identifier = accommodationimporteddetail.Id;

                //We get only the ID
                //We have to load the elaborated data?
                Console.WriteLine("Processing Accommodation " + identifier);

                var accommodationobjectmongo = await LoadDataFromMongo(new RabbitNotifyMessage() { collection = "accommodationdetail", db = "lts", id = identifier });
                var accommodationobjectmongojson = JToken.Parse(accommodationobjectmongo.rawdata);
                var accommodationobjectmongojsonarray = JArray.FromObject(accommodationobjectmongojson);
                JObject accomodationdetail = accommodationobjectmongojsonarray.FirstOrDefault().Value<JObject>();
                
                //Load all XDocuments
                var xmlfiles = LoadXmlFiles(Path.Combine(".\\xml\\"));
                var jsonfiles = await LoadJsonFiles("Features", Path.Combine(".\\json\\"));

                //Parse the Accommodation
                var accommodation = LTSAPI.Parser.AccommodationParser.ParseLTSAccommodation(accomodationdetail["data"].ToObject<LTSAccoData>(), false, xmlfiles, jsonfiles);

                //Parse Rooms LTS
                var accommodationrooms = LTSAPI.Parser.AccommodationParser.ParseLTSAccommodationRoom(accomodationdetail["data"].ToObject<LTSAccoData>(), false, xmlfiles, jsonfiles);

                var accommodationroomshgv = default(IEnumerable<AccommodationRoomLinked>);

                if (accommodationimporteddetail.HasHGVRooms)
                {
                    var accommodationhgvroomlistmongo = await LoadDataFromMongo(new RabbitNotifyMessage() { collection = "accommodationrooms", db = "hgv", id = identifier });
                    var accommodationhgvroomxml = XElement.Load(accommodationobjectmongo.rawdata);

                    //Parse the Accommodation
                    accommodationroomshgv = HGVApi.Parser.AccommodationParser.ParseHGVAccommodationRoom("de", accommodationhgvroomxml, xmlfiles);
                }


                //TODO Check if there are orphaned Rooms and Delete them from the DB
                
                //TODO Let's write the AccoRoomDetail Object? (Or do it on api side?)

                //GET HGV Accommodation Overview  SHOULD A PATCH METHOD IMPLEMENTED? Or should the accommodation data be loaded before?


                //Write all rooms to thee ODH Api and pass referer + use a service account
                foreach(var accommodationroom in accommodationrooms)
                {
                    var apiresponserooms = await odhapiconnector.PushToODHApiCore(accommodation, accommodation.Id, "AccommodationRoom");
                }

                if (accommodationroomshgv != null)
                {
                    var apiresponseroomshgv = await odhapiconnector.PushToODHApiCore(accommodation, accommodation.Id, "AccommodationRoom");
                }

                //Write to the ODH Api and pass referer + use a service account
                var apiresponse = await odhapiconnector.PushToODHApiCore(accommodation, accommodation.Id, "Accommodation");

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

        public IDictionary<string, XDocument> LoadXmlFiles(string directory, string filename)
        {
            //TODO move this files to Database

            IDictionary<string, XDocument> myxmlfiles = new Dictionary<string, XDocument>();
            myxmlfiles.Add(filename, XDocument.Load(directory + filename + ".xml"));
            
            return myxmlfiles;
        }

        public static async Task<IDictionary<string, JArray>> LoadJsonFiles(string directory, string filename)
        {
            IDictionary<string, JArray> myjsonfiles = new Dictionary<string, JArray>();
            myjsonfiles.Add(filename, await LoadFromJsonAndDeSerialize(filename, directory));

            return myjsonfiles;
        }

        public static async Task<JArray> LoadFromJsonAndDeSerialize(string filename, string path)
        {
            using (StreamReader r = new StreamReader(Path.Combine(path, filename + ".json")))
            {
                string json = await r.ReadToEndAsync();

                return JArray.Parse(json) ?? new JArray();
            }
        }

    }

    public class AccommodationFinished()
    {
        public string Id { get; set; }
        public bool HasHGVRooms { get; set; }
    }
}
