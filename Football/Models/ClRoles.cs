using Microsoft.AspNetCore.Identity;

namespace Football.Models
{
    public class ClRoles: IdentityRole
    {

        public string AdminRole = "Admin";
        public string CoachRole = "Coach";

    }
}
