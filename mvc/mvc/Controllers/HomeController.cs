using Microsoft.AspNetCore.Mvc;
using mvc.Models;

namespace mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly Final_Project_DBcontext db;
        public HomeController(Final_Project_DBcontext db)// Κατασκευαστής
        {
            this.db = db;
        }
        public IActionResult Index()// Μέθοδος  για την εμφάνιση της αρχικής σελίδας
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            return View();
        }

        public IActionResult Privacy()// Μέθοδος  για την εμφάνιση της σελίδας Privacy
        {
            return View();
        }
        public IActionResult Singup()// Μέθοδος  για την εμφάνιση της φόρμας εγγραφής
        {
            return View();
        }
        [HttpPost]
        public IActionResult Singup(User UserInfo)// Μέθοδος Singup για την εγγραφή του χρήστη POST
        {
            //Ελενχοι Για το type του χρήστη
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
                HttpContext.Session.SetInt32("UserId", UserInfo.UserId);
                Seller newSeller = new Seller
                {
                    UserId = UserInfo.UserId,

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
        public IActionResult Logout()// Μέθοδος Logout για την αποσύνδεση 
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("Username");
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Login()// Μέθοδος Login για την εμφάνιση της φόρμας σύνδεσης GET
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User UserInfo)// Μέθοδος Login για την σύνδεση του χρήστη POST
        {
            var checkLogin = db.Users
                .Where(x => x.Username.Equals(UserInfo.Username) && x.Password.Equals(UserInfo.Password))
                .FirstOrDefault();
            var user = db.Users.FirstOrDefault(x => x.Username == UserInfo.Username);
            if (checkLogin != null)
            {
                HttpContext.Session.SetString("Username", UserInfo.Username);
                switch (user.Property)// Έλεγχος του τύπου του χρήστη και ανακατεύθυνση στην αντίστοιχη σελίδα
                {
                    case "User":
                        return RedirectToAction("Index", "User");

                    case "Client":
                        var checkLogin2 = db.Clients
                            .Where(x => x.UserId.Equals(UserInfo.UserId)).FirstOrDefault();
                        var client = db.Clients.FirstOrDefault(x => x.UserId == UserInfo.UserId);

                        HttpContext.Session.SetInt32("UserId", user.UserId); 
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