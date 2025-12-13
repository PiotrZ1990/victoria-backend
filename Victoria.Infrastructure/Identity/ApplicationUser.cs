using Microsoft.AspNetCore.Identity;

namespace Victoria.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public int? StudentId { get; set; }
    }
}
