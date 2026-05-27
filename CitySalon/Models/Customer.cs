using System.ComponentModel.DataAnnotations;

namespace CitySalon.Models
{
    /// <summary>
    /// Repræsenterer en kunde i systemet.
    /// Arver fælles egenskaber fra User-klassen.
    /// </summary>
    public class Customer : User
    {
        [Required(ErrorMessage = "Navn skal udfyldes.")]
        [StringLength(100, ErrorMessage = "Navnet må maks være 100 tegn.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefonnummer skal udfyldes.")]
        [Phone(ErrorMessage = "Indtast et gyldigt telefonnummer.")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Telefonnummer skal være 8 cifre.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email skal udfyldes.")]
        [EmailAddress(ErrorMessage = "Indtast en gyldig email.")]
        [StringLength(100, ErrorMessage = "Email må maks være 100 tegn.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Liste med kundens bookinger.
        /// </summary>
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}