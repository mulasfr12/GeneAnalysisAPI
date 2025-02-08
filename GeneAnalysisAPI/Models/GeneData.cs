using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GeneAnalysisAPI.Models
{
    public class GeneData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("patient_id")]
        public string PatientId { get; set; }

        [BsonElement("cancer_cohort")]
        public string CancerCohort { get; set; }

        [BsonElement("gene_name")]
        public string GeneName { get; set; }

        [BsonElement("expression_value")]
        public double ExpressionValue { get; set; }

        // 🔹 New Fields for Clinical Data
        [BsonElement("disease_specific_survival")]
        public int? DiseaseSpecificSurvival { get; set; } // 1 = survived, 0 = did not

        [BsonElement("overall_survival")]
        public int? OverallSurvival { get; set; } // 1 = survived, 0 = did not

        [BsonElement("clinical_stage")]
        public string ClinicalStage { get; set; }
    }
}
