using Microsoft.AspNetCore.Identity;

namespace UserService.Entities
{
    public class User : IdentityUser, IApplicationUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}
