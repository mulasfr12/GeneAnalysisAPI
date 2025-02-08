using MongoDB.Driver;

namespace GeneAnalysisAPI.Services
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;

        public MongoDbService(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDB:ConnectionString"]);
            _database = client.GetDatabase(config["MongoDB:DatabaseName"]);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName) =>
            _database.GetCollection<T>(collectionName);
    }
}
