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
        Task Read<T>(string connectionstring, string queue, string mongoconnection);
    }

    public class ReadMessage : IReadMessage
    {
        private string mongodbconnection;

        public Task Read<T>(string connectionstring, string queue, string mongoconnection)
        {           
            var _rabbitMQServer = new ConnectionFactory() { Uri = new Uri(connectionstring) };
            
            using var connection = _rabbitMQServer.CreateConnection();

            using var channel = connection.CreateModel();

            mongodbconnection = mongoconnection;

            return StartReading<T>(channel, queue);            
        }

        private Task StartReading<T>(IModel channel, string queueName)
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
                ManageMessage<T>(e);
            };

            // Start pushing messages to our consumer
            channel.BasicConsume(queueName, true, consumer);

            Console.WriteLine("Consumer is running");

            //Console.ReadLine();

            return Task.CompletedTask;
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

            return JsonConvert.DeserializeObject<T>(result.rawdata) ;            
        }

        private async Task TransformData<T>(T data)
        {
            //Transformer Logic goes here
            throw new NotImplementedException();
        }
    }


}
