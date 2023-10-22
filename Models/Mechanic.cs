namespace HonestAuto.Models
{
    public class Mechanic
    {
        public int MechanicID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int AttributeName { get; set; }  // Assuming this is an integer based on ERD
        public string Password { get; set; }
        public string Address { get; set; }
    }
}