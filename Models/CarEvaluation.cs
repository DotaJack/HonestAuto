using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HonestAuto.Models
{
    public class CarEvaluation
    {
        [Key]
        public int CarEvaluationID { get; set; } // Unique identifier for a car evaluation

        [ForeignKey("CarID")]
        public int CarID { get; set; } // Foreign key linking to the associated Car entity

        public string MechanicID { get; set; } // ID of the mechanic performing the evaluation (nullable)

        public string? EvaluationStatus { get; set; } // Status of the evaluation (nullable)

        public string? EvaluationSummary { get; set; } // Summary of the car evaluation (nullable)

        public DateTime EvaluationDate { get; set; } = DateTime.UtcNow; // Date and time of the evaluation, defaulting to current UTC time

        public double? CarValue { get; set; } // Estimated value of the car (nullable)

        // This class represents the CarEvaluation entity in the database and defines its properties and relationships.
    }
}