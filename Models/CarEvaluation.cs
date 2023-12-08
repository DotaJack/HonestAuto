using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HonestAuto.Models
{
    public class CarEvaluation
    {
        [Key]
        public int CarEvaluationID { get; set; }

        [ForeignKey("CarID")]
        public int CarID { get; set; }

        public int? MechanicID { get; set; }

        public string? EvaluationStatus { get; set; } // Default is null, no need to set it explicitly

        public string? EvaluationSummary { get; set; } // Default is null

        public DateTime EvaluationDate { get; set; } = DateTime.UtcNow;
        public double? CarValue { get; set; }
    }
}