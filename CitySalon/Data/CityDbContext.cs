using CitySalon.Models;
using Microsoft.EntityFrameworkCore;

namespace CitySalon.Data
{
    /// <summary>
    /// Database context for CitySalon.
    /// Styrer forbindelsen mellem modelklasserne og databasen.
    /// </summary>
    public class CityDbContext : DbContext
    {
        /// <summary>
        /// Opretter en ny database context med de options,
        /// som bliver sat op i Program.cs.
        /// </summary>
        /// <param name="options">Indstillinger til databaseforbindelsen.</param>
        public CityDbContext(DbContextOptions<CityDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Tabel med bookinger.
        /// </summary>
        public DbSet<Booking> Bookings { get; set; }

        /// <summary>
        /// Tabel med kunder.
        /// </summary>
        public DbSet<Customer> Customers { get; set; }

        /// <summary>
        /// Tabel med medarbejdere.
        /// </summary>
        public DbSet<Employee> Employees { get; set; }

        /// <summary>
        /// Tabel med produkter.
        /// </summary>
        public DbSet<Product> Products { get; set; }

        /// <summary>
        /// Tabel med behandlinger.
        /// </summary>
        public DbSet<Treatment> Treatments { get; set; }

        /// <summary>
        /// Tabel med alle brugere.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Tabel med administratorer.
        /// </summary>
        public DbSet<Admin> Admins { get; set; }

        /// <summary>
        /// Tabel med kontaktbeskeder.
        /// </summary>
        public DbSet<ContactMessage> ContactMessages { get; set; }

        /// <summary>
        /// Konfigurerer relationer og databaseindstillinger,
        /// som ikke kun styres af modelklasserne.
        /// </summary>
        /// <param name="modelBuilder">
        /// Bruges til at konfigurere tabeller, relationer og kolonner i databasen.
        /// </param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relation mellem Booking og Employee.
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Employee)
                .WithMany(e => e.Bookings)
                .HasForeignKey(b => b.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            // Relation mellem Booking og Customer.
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Customer)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            // Relation mellem Booking og Treatment.
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Treatment)
                .WithMany(t => t.Bookings)
                .HasForeignKey(b => b.TreatmentId)
                .OnDelete(DeleteBehavior.NoAction);

            // Decimal-præcision for produktpriser.
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            // Decimal-præcision for behandlingspriser.
            modelBuilder.Entity<Treatment>()
                .Property(t => t.Price)
                .HasPrecision(18, 2);
        }
    }
}