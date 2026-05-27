using CitySalon.Helpers;
using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Admin.Employees
{
    /// <summary>
    /// PageModel til visning af alle medarbejdere.
    /// Kun administratorer har adgang til siden.
    /// </summary>
    public class GetAllEmployeesModel : PageModel
    {
        private readonly EmployeeService _employeeService;

        /// <summary>
        /// Liste med alle medarbejdere som vises på Razor-siden.
        /// </summary>
        public List<CitySalon.Models.Employee> Employees { get; set; } = new();

        /// <summary>
        /// Opretter GetAllEmployeesModel med adgang til EmployeeService.
        /// </summary>
        /// <param name="employeeService">
        /// Service til håndtering af medarbejdere.
        /// </param>
        public GetAllEmployeesModel(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Henter alle medarbejdere og viser siden.
        /// </summary>
        /// <returns>
        /// Redirecter til login hvis brugeren ikke er admin.
        /// Ellers vises siden.
        /// </returns>
        public IActionResult OnGet()
        {
            if (!SessionHelper.IsAdmin(HttpContext))
            {
                return RedirectToPage("/Login/LogInPage");
            }

            Employees = _employeeService.GetAll();

            return Page();
        }
    }
}