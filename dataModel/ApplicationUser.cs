using Microsoft.AspNetCore.Identity;

namespace movie_hospital_1.dataModel
{
    public class ApplicationUser:IdentityUser
    {
        public string firstName { get; set; }=string.Empty;
        public string lastName { get; set; } =string.Empty;
        public string? Address { get; set; }
    }
}
