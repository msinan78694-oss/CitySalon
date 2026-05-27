using CitySalon.Helpers;
using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Admin.Employees
{
    /// <summary>
    /// PageModel til oprettelse af medarbejdere.
    /// Kun administratorer har adgang til siden.
    /// </summary>
    public class CreateEmployeeModel : PageModel
    {
        private readonly EmployeeService _employeeService;

        /// <summary>
        /// Binder formularens inputfelter til Employee-objektet ved POST.
        /// </summary>
        [BindProperty]
        public CitySalon.Models.Employee Employee { get; set; } = new CitySalon.Models.Employee();

        /// <summary>
        /// Opretter CreateEmployeeModel med adgang til EmployeeService.
        /// </summary>
        /// <param name="employeeService">Service til håndtering af medarbejdere.</param>
        public CreateEmployeeModel(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Viser siden til oprettelse af medarbejdere.
        /// </summary>
        /// <returns>
        /// Redirecter til login hvis brugeren ikke er admin.
        /// Ellers vises siden.
        /// </returns>
        public IActionResult OnGet()
        {
            if (!CanAccessPage())
            {
                return RedirectToPage("/Login/LogInPage");
            }

            return Page();
        }

        /// <summary>
        /// Opretter en ny medarbejder i systemet.
        /// </summary>
        /// <returns>
        /// Redirecter til medarbejderlisten hvis oprettelsen lykkes.
        /// Returnerer Page() hvis validation eller fejl opstår.
        /// </returns>
        public IActionResult OnPost()
        {
            if (!CanAccessPage())
            {
                return RedirectToPage("/Login/LogInPage");
            }

            // Rollen sættes automatisk.
            Employee.Role = typeof(CitySalon.Models.Employee).Name;

            // Role findes ikke som synligt inputfelt i formularen.
            // Derfor fjernes den fra ModelState.
            ModelState.Remove("Employee.Role");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                _employeeService.Add(Employee);

                return RedirectToPage("/Admin/Employees/GetAllEmployees");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                return Page();
            }
        }

        /// <summary>
        /// Tjekker om brugeren er administrator.
        /// </summary>
        /// <returns>True hvis brugeren har adgang til siden.</returns>
        private bool CanAccessPage()
        {
            return SessionHelper.IsAdmin(HttpContext);
        }
    }
}