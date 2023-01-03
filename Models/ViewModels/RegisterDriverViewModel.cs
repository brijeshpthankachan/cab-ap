namespace CSMS.Models.ViewModels
{
    public class RegisterDriverViewModel
    {
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "LastName")]
        public string? LastName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required]
        [StringLength(25)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(25)]
        [Compare(nameof(Password))]
        [Display(Name = "Confirm Password")]
        public string? ConfirmPassword { get; set; }


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
        [Required]
        public bool IsOnRoad { get; set; } = false;

        [Required] public string CabLocation { get; set; }


         public DateTime BookingDate { get; set; }
         public double Fair { get; set; }
         public string? Source { get; set; }
         public string? Destination { get; set; }
         public string? CurrentUserID { get; set; }
         public string BookingStatus { get; set; }

         public int BookingId { get; set; }
         public string CustomerName { get; set; }


    }
}
