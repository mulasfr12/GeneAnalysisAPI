using System.Net.Http.Json;
using GeneAnalysisUI.Models;


namespace GeneAnalysisUI.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;

        public ApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<GeneData>> GetGeneData(string patientId)
        {
            return await _http.GetFromJsonAsync<List<GeneData>>($"api/gene-expression/{patientId}");
        }
    }
}
