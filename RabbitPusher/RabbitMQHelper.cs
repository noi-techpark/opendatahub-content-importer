// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using TimeZoneConverter;
using GenericHelper;

namespace RabbitPusher
{
    public class RabbitMQSend
    {
        private readonly string rabbitMQconn;

        public RabbitMQSend(string connectionstring)
        {
            rabbitMQconn = connectionstring;
        }

        public void Send(string provider, object obj, string? objectid = null)
        {
            var factory = new ConnectionFactory { Uri = new Uri(rabbitMQconn) };

            using var connection = factory.CreateConnection();
            
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "ingress-q",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,                                 
                                 arguments: null);

            var message = new RabbitIngressMessage()
            {
                id = objectid == null ? Guid.NewGuid().ToString() : objectid,
                provider = provider,
                timestamp = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK"),
                //rawdata = obj
                rawdata = JsonConvert.SerializeObject(obj)
                //Encoding.UTF8.GetBytes(text)
            };

            //var messagebody = RabbitHelper.ConvertToBytes(message);#

            var messagebody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            channel.BasicPublish(exchange: "ingress",
                                 routingKey: provider,
                                 basicProperties: null,
                                 body: messagebody);


            Console.WriteLine(JsonConvert.SerializeObject(new RabbitLog() { id = message.id, routingkey = message.provider }));
        }
    }

    public class RabbitHelper
    {             
        public static byte[] ConvertToBytes(object obj)
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new BsonWriter(ms))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(writer, new { Value = obj });
                    return ms.ToArray();
                }
            }
        }
    }

    public class RabbitLog
    {
        public string id { get; set; }
        public string routingkey { get; set; }        
    }
}
