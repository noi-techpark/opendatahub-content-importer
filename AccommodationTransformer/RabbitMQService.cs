// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

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
        void Read(string connectionstring, string queue, string mongoconnection);
    }

    public abstract class ReadMessage : IReadMessage
    {
        protected string mongodbconnection;
        protected string rabbitmqconnection;

        public void Read(string connectionstring, string queue, string mongoconnection)
        {
            rabbitmqconnection = connectionstring;
            var _rabbitMQServer = new ConnectionFactory() { Uri = new Uri(connectionstring) };

            using var connection = _rabbitMQServer.CreateConnection();

            using var channel = connection.CreateModel();

            mongodbconnection = mongoconnection;

            StartReading(channel, queue);
        }

        private async void StartReading(IModel channel, string queueName)
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
            consumer.Received += (sender, e) =>
            {
                _ = ManageMessage(e);
            };

            // Start pushing messages to our consumer
            channel.BasicConsume(queueName, true, consumer);

            Console.WriteLine("Consumer is running");

            Console.ReadLine();
        }

        private async Task ManageMessage(BasicDeliverEventArgs e)
        {
            var body = e.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("received: " + message);

            var rabbitmessage = JsonConvert.DeserializeObject<RabbitNotifyMessage>(message);

            //Load from MongoDB
            var data = await LoadDataFromMongo(rabbitmessage);

            await TransformData(data);
        }

        private async Task<MongoDBObject> LoadDataFromMongo(RabbitNotifyMessage message)
        {
            MongoDBReader mongoreader = new MongoDBReader(mongodbconnection);
            return await mongoreader.GetFromMongoAsObject(message);
        }

        public virtual async Task TransformData(MongoDBObject data)
        {
            //Transformer Logic goes here
            throw new NotImplementedException();
        }
    }

    #endregion


    public interface IReadAccommodationChanged : IReadMessage
    {

    }

    public class ReadAccommodationChanged : ReadMessage, IReadAccommodationChanged
    {
        public override async Task TransformData(MongoDBObject data)
        {

            var json = JToken.Parse(data.rawdata);

            JArray ridlist = json["data"].Value<JArray>();

            Console.WriteLine("ReadAccommodationChanged called");

            //Push all Rids to accommodation/detail
            //RabbitMQSend rabbitsend = new RabbitMQSend(rabbitmqconnection);
            //var ltsamenities = await ltsapi.AccommodationAmenitiesRequest(null, true);
            //rabbitsend.Send("lts/accommodationamenities", ltsamenities);
        }
    }

    public interface IReadAccommodationDetail : IReadMessage
    {

    }

    public class ReadAccommodationDetail : ReadMessage, IReadAccommodationDetail
    {
        public override async Task TransformData(MongoDBObject data)
        {
            Console.WriteLine("ReadAccommodationDetail called");
        }
    }

}
