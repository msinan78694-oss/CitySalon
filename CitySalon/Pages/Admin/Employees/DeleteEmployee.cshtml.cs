using CitySalon.Helpers;
using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Admin.Employees
{
    /// <summary>
    /// PageModel til sletning af medarbejdere.
    /// Kun administratorer har adgang til siden.
    /// </summary>
    public class DeleteEmployeeModel : PageModel
    {
        private readonly EmployeeService _employeeService;

        /// <summary>
        /// Binder Employee-objektet fra formularen ved POST.
        /// </summary>
        [BindProperty]
        public CitySalon.Models.Employee Employee { get; set; } = new CitySalon.Models.Employee();

        /// <summary>
        /// Opretter DeleteEmployeeModel med adgang til EmployeeService.
        /// </summary>
        /// <param name="employeeService">
        /// Service til håndtering af medarbejdere.
        /// </param>
        public DeleteEmployeeModel(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Henter medarbejderen som skal slettes og viser siden.
        /// </summary>
        /// <param name="id">Id på medarbejderen som skal slettes.</param>
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
        /// Sletter den valgte medarbejder.
        /// </summary>
        /// <returns>
        /// Redirecter til medarbejderlisten efter sletning.
        /// Returnerer Page() hvis der opstår fejl.
        /// </returns>
        public IActionResult OnPost()
        {
            // Tjekker adgang igen ved POST,
            // da brugeren kan forsøge at sende formularen direkte.
            if (!CanAccessPage())
            {
                return RedirectToPage("/Login/LogInPage");
            }

            try
            {
                // Sletter medarbejderen ud fra id'et
                // fra hidden inputfeltet.
                _employeeService.Delete(Employee.Id);

                return RedirectToPage("/Admin/Employees/GetAllEmployees");
            }
            catch (Exception ex)
            {
                // Viser fejlbesked fra service-laget.
                ModelState.AddModelError("", ex.Message);

                // Henter medarbejderen igen,
                // så siden stadig kan vise oplysningerne.
                return LoadEmployeeOrRedirect();
            }
        }

        /// <summary>
        /// Henter medarbejderen igen hvis sletning fejler.
        /// </summary>
        /// <returns>
        /// Returnerer Page() hvis medarbejderen findes.
        /// Ellers redirectes der til medarbejderlisten.
        /// </returns>
        private IActionResult LoadEmployeeOrRedirect()
        {
            try
            {
                Employee = _employeeService.GetById(Employee.Id);

                return Page();
            }
            catch
            {
                return RedirectToPage("/Admin/Employees/GetAllEmployees");
            }
        }

        /// <summary>
        /// Tjekker om brugeren er administrator.
        /// </summary>
        /// <returns>
        /// True hvis brugeren har adgang til siden.
        /// </returns>
        private bool CanAccessPage()
        {
            return SessionHelper.IsAdmin(HttpContext);
        }
    }
}