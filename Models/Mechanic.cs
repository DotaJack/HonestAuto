namespace HonestAuto.Models
{
    public class Mechanic
    {
        public int MechanicID { get; set; } // Unique identifier for a mechanic

        public string FirstName { get; set; } // First name of the mechanic

        public string LastName { get; set; } // Last name of the mechanic

        public string Email { get; set; } // Email address of the mechanic

        public string Password { get; set; } // Password of the mechanic (note: in a real application, sensitive data like passwords should be securely hashed and stored)

        public string Address { get; set; } // Address of the mechanic

        public long PhoneNumber { get; set; } // Phone number of the mechanic

        // This class represents the Mechanic entity in the application and defines its properties.
    }
}