namespace CSMS.Models
{
    public class Booking
    {

        [Key] public int Id { get; set; }
        [Required]
        public string ApplicationUserId { get; set; }
        [Required]
        public ApplicationUser ApplicationUsers { get; set; }
        [Required] public string? UserId { get; set; }
        [Required] public DateTime BookingTime { get; set; }
        [Required] public string? Source { get; set; }
        [Required] public string? Destination { get; set; }
        [Required]
        public string BookingStatus { get; set; }

        [Required]
        public double Fair { get; set; }

    }
}
