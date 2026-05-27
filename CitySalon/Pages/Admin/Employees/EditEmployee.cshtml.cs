using CitySalon.Helpers;
using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Admin.Employees
{
    /// <summary>
    /// PageModel til redigering af medarbejdere.
    /// Kun administratorer har adgang til siden.
    /// </summary>
    public class EditEmployeeModel : PageModel
    {
        private readonly EmployeeService _employeeService;

        /// <summary>
        /// Binder formularens inputfelter til Employee-objektet ved POST.
        /// </summary>
        [BindProperty]
        public CitySalon.Models.Employee Employee { get; set; } = new CitySalon.Models.Employee();

        /// <summary>
        /// Opretter EditEmployeeModel med adgang til EmployeeService.
        /// </summary>
        /// <param name="employeeService">Service til håndtering af medarbejdere.</param>
        public EditEmployeeModel(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Henter medarbejderen som skal redigeres og viser siden.
        /// </summary>
        /// <param name="id">Id på medarbejderen som skal redigeres.</param>
        /// <returns>
        /// Redirecter til login hvis brugeren ikke er admin.
        /// Redirecter til medarbejderlisten hvis medarbejderen ikke findes.
        /// Ellers vises siden.
        /// </returns>
        public IActionResult OnGet(int id)
        {
            if (!CanAccessPage())
            {
                return RedirectToPage("/Login/LogInPage");
            }

            try
            {
                Employee = _employeeService.GetById(id);

                return Page();
            }
            catch (Exception)
            {
                return RedirectToPage("/Admin/Employees/GetAllEmployees");
            }
        }

        /// <summary>
        /// Gemmer ændringer på den valgte medarbejder.
        /// </summary>
        /// <returns>
        /// Redirecter til medarbejderlisten hvis opdateringen lykkes.
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

            // Role og Bookings kommer ikke fra synlige inputfelter i formularen.
            // Derfor fjernes de fra ModelState.
            ModelState.Remove("Employee.Role");
            ModelState.Remove("Employee.Bookings");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                _employeeService.Update(Employee);

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