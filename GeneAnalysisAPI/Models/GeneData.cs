using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

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
    }
}
