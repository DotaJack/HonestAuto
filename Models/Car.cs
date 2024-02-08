using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonestAuto.Models
{
    public class Car
    {
        [Key]
        public int CarID { get; set; } // Unique identifier for a car

        [Required]
        public int BrandId { get; set; } // Brand or manufacturer of the car

        [Required]
        public int ModelId { get; set; } // Model of the car

        [Required]
        [Range(1900, 2100)]
        public int Year { get; set; } // Year of the car, limited to a range between 1900 and 2100

        [Required]
        [Range(0, int.MaxValue)]
        public int Mileage { get; set; } // Mileage of the car, must be non-negative

        [Required]
        public string History { get; set; } // Optional history or additional information about the car

        [DefaultValue("Evaluating")]
        [Required]
        public string Registration { get; set; } // Optional history or additional information about the car

        [Required]
        public string Status { get; set; } // Optional history or additional information about the car

        [Required]
        public string Colour { get; set; } // Optional history or additional information about the car

        [Required]
        public byte[]? CarImage { get; set; } // Binary data representing an image of the car (nullable)

        [Required]
        public string UserID { get; set; } // Foreign key to the User table

        // This class represents the Car entity in the database and defines its properties and constraints.
    }
}