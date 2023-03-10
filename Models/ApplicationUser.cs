using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CSMS.Models
{
    public class ApplicationUser:IdentityUser
    {
        [StringLength(15)] public string? FirstName { get; set; }
        [StringLength(15)] public string? LastName { get; set; }
    }
}
