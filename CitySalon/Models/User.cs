using System.ComponentModel.DataAnnotations;

namespace CitySalon.Models
{
    /// <summary>
    /// Baseklasse for alle brugertyper i systemet.
    /// Indeholder fælles egenskaber som login og rolle.
    /// </summary>
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Brugernavn skal udfyldes.")]
        [StringLength(100, ErrorMessage = "Brugernavnet må maks være 100 tegn.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kodeord skal udfyldes.")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Kodeord skal være mellem 4 og 20 tegn.")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Brugerens rolle i systemet.
        /// F.eks. Admin, Employee eller Customer.
        /// </summary>
        [Required(ErrorMessage = "Rolle skal udfyldes.")]
        [StringLength(10, ErrorMessage = "Rollen må maks være 10 tegn.")]
        public string Role { get; set; } = string.Empty;
    }
}