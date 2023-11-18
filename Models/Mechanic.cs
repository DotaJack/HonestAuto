namespace HonestAuto.Models
{
    public class Mechanic
    {
        public int MechanicID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string Address { get; set; }
        public long PhoneNumber { get; set; }

        // public byte[] ProfileImage { get; set; }
        public List<CarEvaluation> CarEvaluations { get; set; }
    }
}