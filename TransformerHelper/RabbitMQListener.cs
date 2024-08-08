using DataImportHelper;
using Helper;
using MongoDBConnector;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace TransformerHelper
{
    #region Generic Code
    public interface IReadMessage
    {
        void Read(string rabbitconnectionstring, string mongoconnection, List<string> queues, IDictionary<string, DataImport> dataimport, ODHApiConnector odhapiconnector);
    }

    public abstract class ReadMessage : IReadMessage
    {
        protected string mongodbconnection;
        //protected string rabbitmqconnection;
        protected IDictionary<string, DataImport> dataimport;
        protected ODHApiConnector odhapiconnector;

        public void Read(string rabbitconnectionstring, string mongoconnection, List<string> queues, IDictionary<string, DataImport> _dataimport, ODHApiConnector _odhapiconnector)
        {            
            var _rabbitMQServer = new ConnectionFactory() { Uri = new Uri(rabbitconnectionstring) };

            using var connection = _rabbitMQServer.CreateConnection();

            using var channel = connection.CreateModel();

            mongodbconnection = mongoconnection;
            dataimport = _dataimport;

            odhapiconnector = _odhapiconnector;

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
                    if (result)
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
}
