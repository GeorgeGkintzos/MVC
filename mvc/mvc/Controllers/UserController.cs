using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mvc.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Index()// Μέθοδος για την εμφάνιση της αρχικής σελίδας του User
        {
            return View();
        }
    }
}
