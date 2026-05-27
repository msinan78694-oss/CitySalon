using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CitySalon.Models
{
    /// <summary>
    /// Repræsenterer en booking i systemet.
    /// Indeholder information om kunde, behandling,
    /// medarbejder samt tidspunkt for bookingen.
    /// </summary>
    public class Booking
    {
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Du skal vælge en kunde.")]
        [Range(1, int.MaxValue, ErrorMessage = "Du skal vælge en gyldig kunde.")]
        public int CustomerId { get; set; }

        /// <summary>
        /// Den kunde som bookingen tilhører.
        /// </summary>
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        [Required(ErrorMessage = "Du skal vælge en behandling.")]
        [Range(1, int.MaxValue, ErrorMessage = "Du skal vælge en gyldig behandling.")]
        public int TreatmentId { get; set; }

        /// <summary>
        /// Den behandling som er valgt til bookingen.
        /// </summary>
        [ForeignKey("TreatmentId")]
        public Treatment? Treatment { get; set; }

        [Required(ErrorMessage = "Du skal vælge en medarbejder.")]
        [Range(1, int.MaxValue, ErrorMessage = "Du skal vælge en gyldig medarbejder.")]
        public int EmployeeId { get; set; }

        /// <summary>
        /// Den medarbejder som udfører behandlingen.
        /// </summary>
        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }

        [Required(ErrorMessage = "Du skal vælge en starttid.")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Du skal vælge en sluttid.")]
        public DateTime EndTime { get; set; }

        public string? Note { get; set; }

        /// <summary>
        /// Bookingens status.
        /// Standardværdien er Pending.
        /// </summary>
        [Required(ErrorMessage = "Status skal udfyldes.")]
        public string Status { get; set; } = "Pending";
    }
}