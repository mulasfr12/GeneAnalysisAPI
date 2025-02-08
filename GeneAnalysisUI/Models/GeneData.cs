namespace GeneAnalysisUI.Models
{
    public class GeneData
    {
        public string Id { get; set; }
        public string PatientId { get; set; }
        public string CancerCohort { get; set; }
        public string GeneName { get; set; }
        public double ExpressionValue { get; set; }
    }
}
