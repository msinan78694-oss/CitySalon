using System.ComponentModel.DataAnnotations;

namespace CitySalon.Models
{
    /// <summary>
    /// Repræsenterer en medarbejder i systemet.
    /// Arver fælles egenskaber fra User-klassen.
    /// </summary>
    public class Employee : User
    {
        [Required(ErrorMessage = "Navn skal udfyldes.")]
        [StringLength(100, ErrorMessage = "Navnet må maks være 100 tegn.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Liste med medarbejderens bookinger.
        /// </summary>
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}