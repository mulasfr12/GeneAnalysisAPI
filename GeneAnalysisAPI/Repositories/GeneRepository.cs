using GeneAnalysisAPI.Models;
using MongoDB.Driver;

namespace GeneAnalysisAPI.Repositories
{
    public class GeneRepository : IGeneRepository
    {
        private readonly IMongoCollection<GeneData> _geneCollection;
        private readonly IMongoCollection<ClinicalData> _clinicalCollection;

        private readonly HashSet<string> _cGasStingGenes = new()
        {
            "C6orf150", "CCL5", "CXCL10", "TMEM173", "CXCL9", "CXCL11",
            "NFKB1", "IKBKE", "IRF3", "TREX1", "ATM", "IL6", "IL8"
        };
        private readonly MongoClient _client;

        public GeneRepository(IConfiguration config)
        {
            // Ensure that all configuration values exist
            var connectionString = config["MongoDB:ConnectionString"];
            var databaseName = config["MongoDB:DatabaseName"];
            var geneCollectionName = config["MongoDB:GeneCollection"];
            var clinicalCollectionName = config["MongoDB:ClinicalCollection"];

            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(databaseName) ||
                string.IsNullOrEmpty(geneCollectionName) || string.IsNullOrEmpty(clinicalCollectionName))
            {
                throw new ArgumentNullException("One or more MongoDB configuration values are missing.");
            }

            // Connect to MongoDB
            _client = new MongoClient(connectionString);
            var database = _client.GetDatabase(databaseName);

            _geneCollection = database.GetCollection<GeneData>(geneCollectionName);
            _clinicalCollection = database.GetCollection<ClinicalData>(clinicalCollectionName);

            // Test Connection
            var dbList = _client.ListDatabaseNames().ToList();
            Console.WriteLine("Connected to MongoDB. Databases found: " + string.Join(", ", dbList));
        }
        public async Task<List<GeneData>> GetGeneDataWithClinicalInfoAsync(string patientId)
        {
            var geneDataList = await _geneCollection.Find(g => g.PatientId == patientId).ToListAsync();
            var clinicalData = await _clinicalCollection.Find(c => c.PatientId == patientId).FirstOrDefaultAsync();

            if (clinicalData != null)
            {
                foreach (var gene in geneDataList)
                {
                    gene.DiseaseSpecificSurvival = clinicalData.DiseaseSpecificSurvival;
                    gene.OverallSurvival = clinicalData.OverallSurvival;
                    gene.ClinicalStage = clinicalData.ClinicalStage;
                }
            }

            return geneDataList;
        }
        public async Task InsertClinicalDataAsync(List<ClinicalData> clinicalDataList)
        {
            await _clinicalCollection.InsertManyAsync(clinicalDataList);
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
        public async Task InsertGeneDataAsync(List<GeneData> geneDataList)
        {
            var filteredGenes = geneDataList
                .Where(g => _cGasStingGenes.Contains(g.GeneName))
                .ToList();

            if (filteredGenes.Count > 0)
            {
                await _geneCollection.InsertManyAsync(filteredGenes);
            }
        }
    }
}
