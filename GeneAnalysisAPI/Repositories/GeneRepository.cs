using GeneAnalysisAPI.Models;
using MongoDB.Driver;

namespace GeneAnalysisAPI.Repositories
{
    public class GeneRepository : IGeneRepository
    {
        private readonly IMongoCollection<GeneData> _geneCollection;
        private readonly MongoClient _client;

        public GeneRepository(IConfiguration config)
        {
            _client = new MongoClient(config["MongoDB:ConnectionString"]);
            var client = new MongoClient(config["MongoDB:ConnectionString"]);
            var database = client.GetDatabase(config["MongoDB:DatabaseName"]);
            _geneCollection = database.GetCollection<GeneData>(config["MongoDB:GeneCollection"]);

            // Test Connection
            var dbList = _client.ListDatabaseNames().ToList();
            Console.WriteLine("Connected to MongoDB. Databases found: " + string.Join(", ", dbList));
        }

        public async Task<List<GeneData>> GetAllGenesAsync()
        {
            return await _geneCollection.Find(_ => true).ToListAsync();
        }

        public async Task<GeneData> GetGeneByIdAsync(string id)
        {
            return await _geneCollection.Find(g => g.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<GeneData>> GetGenesByPatientIdAsync(string patientId)
        {
            return await _geneCollection.Find(g => g.PatientId == patientId).ToListAsync();
        }

        public async Task InsertGeneAsync(GeneData gene)
        {
            await _geneCollection.InsertOneAsync(gene);
        }

        public async Task UpdateGeneAsync(string id, GeneData updatedGene)
        {
            await _geneCollection.ReplaceOneAsync(g => g.Id == id, updatedGene);
        }

        public async Task DeleteGeneAsync(string id)
        {
            await _geneCollection.DeleteOneAsync(g => g.Id == id);
        }
    }
}
