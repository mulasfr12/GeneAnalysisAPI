using System.Text.Json.Serialization;

namespace GeneAnalysisUI.Models
{
    public class GeneData
    {
        public string Id { get; set; }
        public string PatientId { get; set; }

        [JsonPropertyName("cancerCohort")]
        public string CancerCohort { get; set; }

        [JsonPropertyName("geneName")]
        public string GeneName { get; set; }

        [JsonPropertyName("expressionValue")]
        public double ExpressionValue { get; set; }

        public int? DiseaseSpecificSurvival { get; set; }  // ✅ Must match API's int (1/0)
        public int? OverallSurvival { get; set; }  // ✅ Must match API's int (1/0)


        [JsonPropertyName("clinicalStage")]
        public string ClinicalStage { get; set; }
    }
}
