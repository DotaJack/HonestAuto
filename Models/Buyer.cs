namespace HonestAuto.Models
{
    public class Buyer
    {
        public int BuyerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public long PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }

        public Buyer()
        {
            FirstName = "";
            LastName = "";
            Email = "";
            Password = "";
            Address = "";
        }
    }
}