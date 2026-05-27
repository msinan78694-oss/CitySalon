namespace CitySalon.Helpers
{
    /// <summary>
    /// Helper-klasse til håndtering af session og brugerroller.
    /// Indeholder hjælpe-metoder til at tjekke login og adgangsroller.
    /// </summary>
    public static class SessionHelper
    {
        /// <summary>
        /// Tjekker om brugeren er logget ind.
        /// </summary>
        /// <param name="context">Det aktuelle HttpContext.</param>
        /// <returns>
        /// True hvis der findes et UserId i sessionen,
        /// ellers false.
        /// </returns>
        public static bool IsLoggedIn(HttpContext context)
        {
            return context.Session.GetInt32("UserId") != null;
        }

        /// <summary>
        /// Tjekker om den loggede bruger har rollen Admin.
        /// </summary>
        /// <param name="context">Det aktuelle HttpContext.</param>
        /// <returns>True hvis brugeren er Admin.</returns>
        public static bool IsAdmin(HttpContext context)
        {
            return context.Session.GetString("Role") == "Admin";
        }

        /// <summary>
        /// Tjekker om den loggede bruger har rollen Employee.
        /// </summary>
        /// <param name="context">Det aktuelle HttpContext.</param>
        /// <returns>True hvis brugeren er Employee.</returns>
        public static bool IsEmployee(HttpContext context)
        {
            return context.Session.GetString("Role") == "Employee";
        }

        /// <summary>
        /// Tjekker om den loggede bruger har rollen Customer.
        /// </summary>
        /// <param name="context">Det aktuelle HttpContext.</param>
        /// <returns>True hvis brugeren er Customer.</returns>
        public static bool IsCustomer(HttpContext context)
        {
            return context.Session.GetString("Role") == "Customer";
        }

        /// <summary>
        /// Tjekker om brugeren enten er Admin eller Employee.
        /// </summary>
        /// <param name="context">Det aktuelle HttpContext.</param>
        /// <returns>
        /// True hvis brugeren har rollen Admin eller Employee.
        /// </returns>
        public static bool IsAdminOrEmployee(HttpContext context)
        {
            string? role = context.Session.GetString("Role");

            return role == "Admin" || role == "Employee";
        }
    }
}