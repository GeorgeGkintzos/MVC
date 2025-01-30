using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using mvc.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace mvc.Controllers
{
    public class AdminController : Controller
    {
        // Προσθήκη του Final_Project_DBcontext
        private readonly Final_Project_DBcontext _context;
        // Κατασκευαστής
        public AdminController(Final_Project_DBcontext context)
        {
            _context = context;
        }
        // Μέθοδος Index για την εμφάνιση της αρχικής σελίδας του Admin
        public IActionResult Index()
        {
            return View();
        }
        // Μέθοδος για την εμφάνιση των χρηστών που είναι Sellers
        public IActionResult SellerList()
        {
            List<Models.User> users = _context.Users.Where(u => u.Property == "Seller").ToList();
            return View(users);
        }
        // Μέθοδος για την εμφάνιση των χρηστών που είναι Users (και όχι Sellers)
        public IActionResult RemoveFromSellersList(int id)
        {
            // Έλεγχος για το id που λαμβάνεις
            Console.WriteLine($"Removing seller with id: {id}");

            // Αναζήτηση στο Sellers με βάση το UserId 
            var seller = _context.Sellers.FirstOrDefault(s => s.UserId == id);
            if (seller == null)
            {
                return NotFound("Ο πωλητής δεν βρέθηκε.");
            }

            // Διαγραφή του πωλητή
            _context.Sellers.Remove(seller);

            // Ενημέρωση του UserProperty 
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user != null)
            {
                user.Property = "User";
            }
            // Αποθήκευση των αλλαγών στη βάση
            _context.SaveChanges();
            return RedirectToAction("SellerList", "Admin");
        }
        // Μέθοδος για την δημιουργία sellers
        public IActionResult MakeSeller(int id)
        {
            Models.Seller s = new Models.Seller();
            s.UserId = id;
            Models.User user = _context.Users.FirstOrDefault(u => u.UserId == id);
            user.Property = "Seller";
            _context.Sellers.Add(s);
            _context.SaveChanges();
            return RedirectToAction("CreateSeller", "Admin");
        }
        // Μέθοδος που επιστρέφεί 
        public IActionResult CreateSeller()
        {
            List<Models.User> users = _context.Users.Where(u => u.Property == "User").ToList();
            return View(users);
        }
        
        public IActionResult CreateProgram()// Μέθοδος για την δημιουργία προγράμματος
        {
            return View();
        }
        public IActionResult EditProgram()// Μέθοδος για την επεξεργασία προγράμματος
        {
            return View(_context.Programs.ToList());
        }
        [HttpGet]
        public IActionResult CreateNewProgram()// Μέθοδος για την δημιουργία νέου προγράμματος
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateNewProgram(Models.Program program)// Μέθοδος για την δημιουργία νέου προγράμματος
        {
            _context.Programs.Add(program);
            _context.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }
        [HttpGet]
        public IActionResult Edit(string id)// Μέθοδος για την επεξεργασία προγράμματος GET
        {
            
            var program = _context.Programs.FirstOrDefault(p => p.ProgramName == id);

            if (program == null)
            {
                return RedirectToAction("Index", "Home"); 
            }

            return View(program); // Pass the retrieved Program object to the view
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Models.Program program)// Μέθοδος για την επεξεργασία προγράμματος POST
        {
            if (ModelState.IsValid)
            {
                // αποθήκευση των αλλαγών στη βάση
                _context.Update(program);
                _context.SaveChanges();
                return RedirectToAction("EditProgram", "Admin");
            }
            return View(program);
        }

    }

}