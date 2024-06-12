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
        void Read<T>(string connectionstring, string queue, string mongoconnection);
    }

    public abstract class ReadMessage : IReadMessage
    {
        private string mongodbconnection;

        public void Read<T>(string connectionstring, string queue, string mongoconnection)
        {
            var _rabbitMQServer = new ConnectionFactory() { Uri = new Uri(connectionstring) };

            using var connection = _rabbitMQServer.CreateConnection();

            using var channel = connection.CreateModel();

            mongodbconnection = mongoconnection;

            StartReading<T>(channel, queue);
        }

        private async void StartReading<T>(IModel channel, string queueName)
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
                ManageMessage<T>(e);
            };

            // Start pushing messages to our consumer
            channel.BasicConsume(queueName, true, consumer);

            Console.WriteLine("Consumer is running");

            Console.ReadLine();
        }

        private void ManageMessage<T>(BasicDeliverEventArgs e)
        {
            var body = e.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("received: " + message);

            var rabbitmessage = JsonConvert.DeserializeObject<RabbitNotifyMessage>(message);

            //Load from MongoDB
            var data = LoadDataFromMongo<T>(rabbitmessage);

            TransformData(data);
        }

        private async Task<T> LoadDataFromMongo<T>(RabbitNotifyMessage message)
        {
            MongoDBReader mongoreader = new MongoDBReader(mongodbconnection);
            var result = await mongoreader.GetFromMongoAsObject(message);

            return JsonConvert.DeserializeObject<T>(result.rawdata);
        }

        public virtual async Task TransformData<T>(T data)
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
        public override async Task TransformData<JObject>(JObject data)
        {
            Console.WriteLine("ReadAccommodationChanged called");
        }
    }

    public interface IReadAccommodationDetail : IReadMessage
    {

    }

    public class ReadAccommodationDetail : ReadMessage, IReadAccommodationDetail
    {
        public override async Task TransformData<JObject>(JObject data)
        {
            Console.WriteLine("ReadAccommodationDetail called");
        }
    }

}
