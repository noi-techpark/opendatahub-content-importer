// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Helper;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDBConnector
{
    public class MongoDBReader
    {
        private readonly string mongodbconnection;

        public MongoDBReader(string connection)
        {
            mongodbconnection = connection;
        }

        public async Task<MongoDBObject> GetFromMongoAsObject(RabbitNotifyMessage rabbitmessage)
        {
            return await GetFromMongoAsObject(rabbitmessage.id, rabbitmessage.db, rabbitmessage.collection);
        }

        public async Task<MongoDBObject> GetFromMongoAsObject(string id, string db, string collection)
        {
            try
            {
                var data = await GetFromMongo(id, db, collection);
                var converted = BsonSerializer.Deserialize<MongoDBObject>(data);                

                return converted;
            }
            catch (Exception ex)
            {
                return default(MongoDBObject);
            }
        }


        private async Task<BsonDocument> GetFromMongo(string id, string db, string collection)
        {
            try
            {
                var mclient = new MongoClient(mongodbconnection);
                var mdatabase = mclient.GetDatabase(db);

                var mongocollection = mdatabase.GetCollection<BsonDocument>(collection);

                var mongoid = new ObjectId(id);

                var obj = await mongocollection.Find($"{{ _id: ObjectId('{mongoid}') }}").SingleAsync();

                return obj;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }

    public class MongoDBObject
    {
        [BsonId]
        public ObjectId _id { get; set; }
        
        public string id { get; set; }
        public string provider { get; set; }
        public string timestamp { get; set; }
        //public object Rawdata { get; set; }
        public string rawdata { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime bsontimestamp { get; set; }
    }
}
