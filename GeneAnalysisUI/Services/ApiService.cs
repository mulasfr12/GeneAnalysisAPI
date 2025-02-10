using System.Net.Http;
using System.Net.Http.Json;
using GeneAnalysisUI.Models;  // ✅ Ensure correct namespace

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
            try
            {
                var response = await _http.GetAsync($"api/gene-expression/patient/{patientId}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"API returned error: {response.StatusCode}");
                    return new List<GeneData>();  // ✅ Return empty list if error
                }

                var result = await response.Content.ReadFromJsonAsync<List<GeneData>>();
                Console.WriteLine($"✅ API Response: Found {result?.Count} records for Patient ID: {patientId}");

                return result ?? new List<GeneData>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error fetching data: {ex.Message}");
                return new List<GeneData>();  // ✅ Prevent crashes
            }
        }

    }
}
