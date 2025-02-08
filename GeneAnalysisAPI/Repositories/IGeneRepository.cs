using GeneAnalysisAPI.Models;

namespace GeneAnalysisAPI.Repositories
{
    public interface IGeneRepository
    {
        Task<List<GeneData>> GetAllGenesAsync();
        Task<GeneData> GetGeneByIdAsync(string id);
        Task<List<GeneData>> GetGenesByPatientIdAsync(string patientId);
        Task InsertGeneAsync(GeneData gene);
        Task UpdateGeneAsync(string id, GeneData updatedGene);
        Task DeleteGeneAsync(string id);
    }
}
