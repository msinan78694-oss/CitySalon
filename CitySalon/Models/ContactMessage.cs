using System.ComponentModel.DataAnnotations;

namespace CitySalon.Models
{
    /// <summary>
    /// Repræsenterer en kontaktbesked sendt fra kontaktformularen.
    /// </summary>
    public class ContactMessage
    {
        [Key]
        public int ContactMessageId { get; set; }

        [Required(ErrorMessage = "Navn skal udfyldes.")]
        [StringLength(100, ErrorMessage = "Navnet må maks være 100 tegn.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email skal udfyldes.")]
        [EmailAddress(ErrorMessage = "Indtast en gyldig email.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Besked skal udfyldes.")]
        [StringLength(1000, ErrorMessage = "Beskeden må maks være 1000 tegn.")]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Tidspunktet hvor beskeden blev sendt.
        /// </summary>
        public DateTime SentAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Viser om beskeden er blevet læst af en administrator.
        /// </summary>
        public bool IsRead { get; set; }
    }
}