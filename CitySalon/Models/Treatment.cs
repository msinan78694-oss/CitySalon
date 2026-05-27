using System.ComponentModel.DataAnnotations;

namespace CitySalon.Models
{
    /// <summary>
    /// Repræsenterer en behandling i systemet.
    /// </summary>
    public class Treatment
    {
        public int TreatmentId { get; set; }

        [Required(ErrorMessage = "Navn på behandling skal udfyldes.")]
        [StringLength(100, ErrorMessage = "Navnet må maks være 100 tegn.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Behandlingens varighed i minutter.
        /// </summary>
        [Required(ErrorMessage = "Varighed skal udfyldes.")]
        [Range(1, 500, ErrorMessage = "Varighed skal være større end 0.")]
        public int DurationInMinutes { get; set; }

        [Required(ErrorMessage = "Kategori skal udfyldes.")]
        [StringLength(100, ErrorMessage = "Kategori må maks være 100 tegn.")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pris skal udfyldes.")]
        [Range(1, 10000, ErrorMessage = "Pris skal være større end 0.")]
        public decimal Price { get; set; }

        /// <summary>
        /// Liste med bookinger der bruger behandlingen.
        /// </summary>
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}