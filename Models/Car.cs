using System.ComponentModel.DataAnnotations;

namespace HonestAuto.Models
{
    public class Car
    {
        [Key]
        public int CarID { get; set; } // Unique identifier for a car

        [Required]
        [StringLength(100)]
        public string BrandId { get; set; } // Brand or manufacturer of the car

        [Required]
        [StringLength(100)]
        public string ModelId { get; set; } // Model of the car

        [Required]
        [Range(1900, 2100)]
        public int Year { get; set; } // Year of the car, limited to a range between 1900 and 2100

        [Required]
        [Range(0, int.MaxValue)]
        public int Mileage { get; set; } // Mileage of the car, must be non-negative

        public string History { get; set; } // Optional history or additional information about the car

        public byte[]? CarImage { get; set; } // Binary data representing an image of the car (nullable)

        [Required]
        public string UserID { get; set; } // Foreign key to the User table

        [Required]
        public string Registration { get; set; } // Foreign key to the User table

        [Required]
        public string Status { get; set; } // Foreign key to the User table

        [Required]
        public string Colour { get; set; } // Foreign key to the User table
    }
}