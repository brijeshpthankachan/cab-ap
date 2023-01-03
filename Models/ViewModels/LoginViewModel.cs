
namespace CSMS.Models.ViewModels;

public class LoginViewModel
{
    [StringLength(50)]
    [Required]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [StringLength(25)]
    [Display(Name = "Password")]

    public string Password { get; set; }
}