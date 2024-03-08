using System.ComponentModel.DataAnnotations;

namespace HonestAuto.Models
{
    public class UserCountsViewModel
    {
        public int TotalUsersCount { get; set; }
        public List<RoleUserCountViewModel> UsersPerRole { get; set; }

        public class RoleUserCountViewModel
        {
            public string RoleName { get; set; }
            public int UserCount { get; set; }
        }
    }
}