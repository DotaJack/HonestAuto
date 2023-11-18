using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HonestAuto.Models;

public class CarEvaluation
{
    [Key]
    public int CarEvaluationID { get; set; }

    public int CarID { get; set; }

    public int MechanicID { get; set; }

    public string EvaluationStatus { get; set; } // Default is null, no need to set it explicitly

    public string EvaluationSummary { get; set; } // Default is null

    public DateTime EvaluationDate { get; set; } = DateTime.UtcNow; // Consider using UTC date
}