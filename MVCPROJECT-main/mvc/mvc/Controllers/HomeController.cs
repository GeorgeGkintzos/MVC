using Microsoft.AspNetCore.Mvc;
using mvc.Models;

namespace mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly Final_Project_DBcontext db;
        public HomeController(Final_Project_DBcontext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            // Retrieve username from session
            ViewBag.Username = HttpContext.Session.GetString("Username");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Singup()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Singup(User UserInfo)
        {
            if (db.Users.Any(x => x.Username == UserInfo.Username))
            {
                ViewBag.Notification = "this account already existed";
                return View();
            }
            else if (UserInfo.Property == "Admin")
            {
                db.Users.Add(UserInfo);
                db.SaveChanges();
                HttpContext.Session.SetString("Username", UserInfo.Username);
                return RedirectToAction("Index", "Admin");
            }
            else if (UserInfo.Property == "Seller")
            {
                db.Users.Add(UserInfo);
                db.SaveChanges();

                // Εκχώρηση του UserId στην περίοδο συνεδρίας (Session) μετά την αποθήκευση
                HttpContext.Session.SetInt32("UserId", UserInfo.UserId); // Αποθήκευση του UserId στο session

                // Δημιουργία νέας εγγραφής στον πίνακα Sellers
                Seller newSeller = new Seller
                {
                    UserId = UserInfo.UserId, // Σύνδεση με τον χρήστη

                };

                db.Sellers.Add(newSeller);
                db.SaveChanges();
            }
            else if (UserInfo.Property == "Client")
            {
                db.Users.Add(UserInfo);
                db.SaveChanges();

                Client newClient = new Client
                {
                    UserId = UserInfo.UserId,
                };

                db.Clients.Add(newClient);
                db.SaveChanges();
                return RedirectToAction("EditClient", "Client");
            }
            else if (UserInfo.Property == "User")
            {
                db.Users.Add(UserInfo);
                db.SaveChanges();
                HttpContext.Session.SetString("Username", UserInfo.Username);
                return RedirectToAction("Index", "User");
            }
            return RedirectToAction("Index", "Home");

        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("Username");
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User UserInfo)
        {
            var checkLogin = db.Users
                .Where(x => x.Username.Equals(UserInfo.Username) && x.Password.Equals(UserInfo.Password))
                .FirstOrDefault();
            var user = db.Users.FirstOrDefault(x => x.Username == UserInfo.Username);
            if (checkLogin != null)
            {
                HttpContext.Session.SetString("Username", UserInfo.Username);
                switch (user.Property)
                {
                    case "User":
                        return RedirectToAction("Index", "User");

                    case "Client":
                        var checkLogin2 = db.Clients
                            .Where(x => x.UserId.Equals(UserInfo.UserId)).FirstOrDefault();
                        var client = db.Clients.FirstOrDefault(x => x.UserId == UserInfo.UserId);

                        HttpContext.Session.SetInt32("UserId", user.UserId); // Αποθήκευση του UserId στη συνεδρία
                        return RedirectToAction("Index", "Client");

                    case "Admin":
                        return RedirectToAction("Index", "Admin");

                    case "Seller":
                        var seller = db.Sellers.FirstOrDefault(s => s.UserId == checkLogin.UserId);
                        if (seller != null)
                        {
                            return RedirectToAction("Index", "Seller");
                        }
                        else
                        {
                            ViewBag.Notification = "No associated seller account found.";
                            break;
                        }

                    default:
                        ViewBag.Notification = "Invalid user role.";
                        break;
                }
            }
            else
            {
                ViewBag.Notification = "Wrong Username Or Password";
            }
            return View();

        }
    }
}