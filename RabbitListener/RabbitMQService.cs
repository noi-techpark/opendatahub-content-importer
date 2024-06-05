// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using MongoDBConnector;
using Helper;

namespace RabbitListener
{    
    public interface IReadMessage
    {
        void Read(string connectionstring, string queue, string mongoconnection);
    }

    public class ReadMessage : IReadMessage
    {
        private string mongodbconnection;

        public void Read(string connectionstring, string queue, string mongoconnection)
        {           
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
            consumer.Received += (sender, e) => {
                ManageMessage(e);
            };

            // Start pushing messages to our consumer
            channel.BasicConsume(queueName, true, consumer);

            Console.WriteLine("Consumer is running");            

            Console.ReadLine();
        }

        private void ManageMessage(BasicDeliverEventArgs e)
        {
            var body = e.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("received: " + message);

            var rabbitmessage = JsonConvert.DeserializeObject<RabbitNotifyMessage>(message);

            //Load from MongoDB
            var task = LoadDataFromMongo(rabbitmessage);
        }

        private async Task LoadDataFromMongo(RabbitNotifyMessage message)
        {
            MongoDBReader mongoreader = new MongoDBReader(mongodbconnection);
            var result = await mongoreader.GetFromMongoAsObject(message);

            var rawdata = JsonConvert.DeserializeObject<TestObject>(result.rawdata);

            Console.WriteLine("object from mongo:" + JsonConvert.SerializeObject(rawdata));
        }
    }


}
