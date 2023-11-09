namespace HonestAuto.Models
{
    public class CarEvaluation
    {
        public int CarEvaluationID { get; set; }
        public int CarID { get; set; }
        public int MechanicID { get; set; }
        public int CarValue { get; set; }
        public string Image { get; set; }
        public string EvaluationStatus { get; set; }
        public string EvaluationSummary { get; set; }
        public DateTime EvaluationDate { get; set; }

        public string EvaluationTime { get; set;}
    }
}