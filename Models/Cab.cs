
namespace CSMS.Models
{
    public class Cab
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ApplicationUserID { get; set; }

        public ApplicationUser? ApplicationUsers { get; set; }
        [Required]
        public string? CabName { get; set; }
        [Required]
        public string? CabType { get; set; }
        [Required]
        public string? LicenseNumber { get; set; }
        [Required]
        public string? RcNumber { get; set; }

        [Required] public string? CabLocation { get; set; }

        [Required]
        public bool IsOnRoad { get; set; } = false;
    }
}
