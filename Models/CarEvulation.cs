using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HonestAuto.Models;

public class CarEvaluation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CarEvaluationID { get; set; }

    [Required]
    public int CarID { get; set; }

    [Required]
    public int MechanicID { get; set; }

    public string EvaluationStatus { get; set; } // Default is null, no need to set it explicitly

    public string EvaluationSummary { get; set; } // Default is null

    [Required]
    public DateTime EvaluationDate { get; set; } = DateTime.UtcNow; // Consider using UTC date

    // Navigation properties
    [ForeignKey("CarID")]
    public virtual Car Car { get; set; }

    [ForeignKey("MechanicID")]
    public virtual Mechanic Mechanic { get; set; }
}