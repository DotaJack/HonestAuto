namespace HonestAuto.Models
{
    public class CarViewModel
    {
        public int CarID { get; set; }
        public string BrandId { get; set; }
        public string BrandName { get; set; }
        public string ModelId { get; set; }
        public string ModelName { get; set; }
        public int Year { get; set; }
        public int Mileage { get; set; }
        public string History { get; set; }
        public string UserID { get; set; }
        public string Registration { get; set; } // Foreign key to the User table

        public string Status { get; set; } // Foreign key to the User table

        public string Colour { get; set; } // Foreign key to the User table
        public byte[] CarImage { get; set; }
    }
}