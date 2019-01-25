using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SimpleBot.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SimpleBot.MongoDB
{
    public class DAO
    {
        private MongoClient client;

        public DAO()
        {
            string connString = "mongodb://localhost:27017";
            client = new MongoClient(connString);

            RegisterAccess();
        }

        public void RegisterAccess()
        {
            //BUSCAR
            var collection = client.GetDatabase("bot").GetCollection<BsonDocument>("access");
            var values = collection.Find("{ user: { $eq: " + Environment.MachineName + " } }").ToList();

            var db = client.GetDatabase("bot");
            var tb = db.GetCollection<BsonDocument>("access");

            int quantity = values != null ? values[0]["quantity"].ToInt32() : -1;
            quantity++;

            //ATUALIZAR
            BsonDocument bson = new BsonDocument()
                {
                    { "user", Environment.MachineName },
                    { "lastaccess",  DateTime.Now.ToString()},
                    { "quantity", quantity }
                };
            tb.UpdateOne("{ user: { $eq: " + Environment.MachineName + " } }", bson, new UpdateOptions() { IsUpsert = true });
        }

        public void Insert(SimpleMessage _simpleMessage)
        {
            try
            {
                var db = client.GetDatabase("bot");
                var tb = db.GetCollection<BsonDocument>("transcript");

                BsonDocument bson = new BsonDocument()
            {
                { "id", _simpleMessage.Id },
                { "user",  _simpleMessage.User},
                { "message", _simpleMessage.Text }
            };

                tb.InsertOne(bson);
            }
            catch (Exception ex)
            {
                //...
            }
        }

        public static async Task<List<BsonDocument>> GetValues(Task<IAsyncCursor<BsonDocument>> queryTask)
        {
            var cursor = await queryTask;
            return await cursor.ToListAsync<BsonDocument>();
        }
    }
}