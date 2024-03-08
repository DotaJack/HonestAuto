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
        public string UserEmail { get; set; }
        public string Registration { get; set; }
        public string Status { get; set; }
        public string Colour { get; set; }
        public byte[] CarImage { get; set; }
        public bool IsEvaluationComplete { get; set; }
        public double? CarValue { get; set; }
        public int CarEvaluationID { get; set; }
        public int CarCount { get; set; }
    }
}