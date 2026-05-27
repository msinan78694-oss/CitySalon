using System.ComponentModel.DataAnnotations;

namespace CitySalon.Models
{
    /// <summary>
    /// Repræsenterer et produkt i systemet.
    /// </summary>
    public class Product
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Produktnavn skal udfyldes.")]
        [StringLength(100, ErrorMessage = "Produktnavnet må maks være 100 tegn.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pris skal udfyldes.")]
        [Range(1, 10000, ErrorMessage = "Pris skal være større end 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Type skal udfyldes.")]
        [StringLength(100, ErrorMessage = "Type må maks være 100 tegn.")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "Størrelse skal udfyldes.")]
        [StringLength(50, ErrorMessage = "Størrelse må maks være 50 tegn.")]
        public string Size { get; set; } = string.Empty;

        [Required(ErrorMessage = "Brand skal udfyldes.")]
        [StringLength(100, ErrorMessage = "Brand må maks være 100 tegn.")]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Beskrivelse skal udfyldes.")]
        [StringLength(1000, ErrorMessage = "Beskrivelsen må maks være 1000 tegn.")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Sti til produktets billede.
        /// </summary>
        [StringLength(300, ErrorMessage = "Billedstien må maks være 300 tegn.")]
        public string ImageUrl { get; set; } = string.Empty;
    }
}