using CitySalon.Data;
using CitySalon.Models;
using CitySalon.Repositories;
using CitySalon.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Tilføjer databaseforbindelse.
builder.Services.AddDbContext<CityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Tilføjer Razor Pages og Session.
builder.Services.AddRazorPages();
builder.Services.AddSession();

// Tilføjer repositories.
// Her registreres interfaces og deres konkrete repository-klasser,
// så de kan bruges med dependency injection.
builder.Services.AddScoped<IProductRepository, ProductRepository>(); // Anvender Async kode og implementerer IRepo, derfor den står samlet med de generiske klasser.
builder.Services.AddScoped<IRepository<Customer>, CustomerRepository>();
builder.Services.AddScoped<IRepository<Employee>, EmployeeRepository>();
builder.Services.AddScoped<IRepository<Treatment>, TreatmentRepository>();

builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IContactMessageRepository, ContactMessageRepository>();

// Tilføjer services.
// Services indeholder forretningslogik og bruger repositories til databasearbejde.
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<TreatmentService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ContactMessageService>();

var app = builder.Build();

// Hvis appen ikke kører i development,
// bruges en generel fejlside.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

// Gør det muligt at bruge statiske filer,
// fx CSS, JavaScript og billeder fra wwwroot.
app.UseStaticFiles();

// Aktiverer routing,
// så Razor Pages kan finde den rigtige side ud fra URL'en.
app.UseRouting();

// Aktiverer session,
// så projektet kan gemme fx UserId, UserName og Role.
app.UseSession();

// Aktiverer authorization middleware.
app.UseAuthorization();

// Mapper Razor Pages,
// så alle .cshtml-sider kan bruges som routes.
app.MapRazorPages();

// Starter applikationen.
app.Run();