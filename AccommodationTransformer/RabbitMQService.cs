// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

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

namespace AccommodationTransformer
{
    #region Generic Code
    public interface IReadMessage
    {
        void Read(string connectionstring, string mongoconnection, List<string> queues, IDictionary<string, DataImport> dataiport);
    }

    public abstract class ReadMessage : IReadMessage
    {
        protected string mongodbconnection;
        protected string rabbitmqconnection;
        protected IDictionary<string, DataImport> dataimport;

        public void Read(string connectionstring, string mongoconnection, List<string> queues, IDictionary<string, DataImport> _dataimport)
        {
            rabbitmqconnection = connectionstring;
            var _rabbitMQServer = new ConnectionFactory() { Uri = new Uri(connectionstring) };

            using var connection = _rabbitMQServer.CreateConnection();

            using var channel = connection.CreateModel();

            mongodbconnection = mongoconnection;
            dataimport = _dataimport;

            StartReading(channel, queues);
        }

        private async void StartReading(IModel channel, List<string> queues)
        {
            foreach (var queueName in queues)
            {
                // connect to the queue
                channel.QueueDeclare(queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                // Consumer definition
                var consumer = new EventingBasicConsumer(channel);

                // Definition of event when the Consumer gets a message
                consumer.Received += async (sender, e) =>
                {
                    var result = await ManageMessage(e);
                    if(result)
                        channel.BasicAck(e.DeliveryTag, false);
                };

                // Start pushing messages to our consumer
                channel.BasicConsume(queueName, false, consumer);

                Console.WriteLine("Consumer is running");
            }
            Console.ReadLine();
        }

        private async Task<bool> ManageMessage(BasicDeliverEventArgs e)
        {
            var body = e.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("received: " + message);

            var rabbitmessage = JsonConvert.DeserializeObject<RabbitNotifyMessage>(message);

            //Load from MongoDB
            var data = await LoadDataFromMongo(rabbitmessage);

            return await TransformData(data, e.RoutingKey);            
        }

        private async Task<MongoDBObject> LoadDataFromMongo(RabbitNotifyMessage message)
        {
            MongoDBReader mongoreader = new MongoDBReader(mongodbconnection);
            return await mongoreader.GetFromMongoAsObject(message);
        }

        public virtual async Task<bool> TransformData(MongoDBObject data, string routingkey)
        {
            //Transformer Logic goes here
            throw new NotImplementedException();
        }
    }

    #endregion


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
    }
}
