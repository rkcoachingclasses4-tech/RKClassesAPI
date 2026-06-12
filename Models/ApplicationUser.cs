using Microsoft.AspNetCore.Identity;

namespace RKClassesApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Add custom properties if needed (e.g., FullName, Address)
        public string? FullName { get; set; }
    }
}
