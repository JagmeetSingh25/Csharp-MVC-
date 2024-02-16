using Microsoft.AspNetCore.Mvc;

namespace Assignment2.Controllers
{
    public class AbstractBaseController : Controller
    {
        public void SetWelcome()
        {
            const string cookieName = "fisrtVisitDate";

            var firstVisitDate = HttpContext.Request.Cookies.ContainsKey(cookieName) && DateTime.TryParse(HttpContext.Request.Cookies[cookieName], out var parseDate)
            ? parseDate : DateTime.Now;


            var welcomeMessage = HttpContext.Request.Cookies.ContainsKey(cookieName)
                ? $"Welcome Back! You first used this app on{firstVisitDate.ToString("MMM dd, yyyy")}"
                : "Hey, Welcome to the Event Manager App!";

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30),
            };

            HttpContext.Response.Cookies.Append(cookieName, firstVisitDate.ToString(), cookieOptions);

            ViewData["WelcomeMessage"] = welcomeMessage;


        }
    }
}
