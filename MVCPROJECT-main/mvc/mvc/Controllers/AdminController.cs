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
        private readonly Final_Project_DBcontext _context;

        public AdminController(Final_Project_DBcontext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SellerList()
        {
            List<Models.User> users = _context.Users.Where(u => u.Property == "Seller").ToList();
            return View(users);
        }
        public IActionResult RemoveFromSellersList(int id)
        {
            // Έλεγχος για το id που λαμβάνεις
            Console.WriteLine($"Removing seller with id: {id}");

            // Αναζήτηση στο Sellers με βάση το UserId (αν υπάρχει σύνδεση με το UserId)
            var seller = _context.Sellers.FirstOrDefault(s => s.UserId == id);

            if (seller == null)
            {
                return NotFound("Ο πωλητής δεν βρέθηκε.");
            }

            // Διαγραφή του πωλητή
            _context.Sellers.Remove(seller);

            // Ενημέρωση του UserProperty σε "User"
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user != null)
            {
                user.Property = "User";
            }

            _context.SaveChanges();
            return RedirectToAction("SellerList", "Admin");
        }

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
        public IActionResult CreateSeller()
        {
            List<Models.User> users = _context.Users.Where(u => u.Property == "User").ToList();
            return View(users);
        }

        public IActionResult CreateProgram()
        {
            return View();
        }
        public IActionResult EditProgram()
        {
            return View(_context.Programs.ToList());
        }
        [HttpGet]
        public IActionResult CreateNewProgram()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateNewProgram(Models.Program program)
        {
            _context.Programs.Add(program);
            _context.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }
        [HttpGet]
        public IActionResult Edit(string id)
        {
            // Example: Replace with your actual data retrieval logic
            var program = _context.Programs.FirstOrDefault(p => p.ProgramName == id);

            if (program == null)
            {
                return RedirectToAction("Index", "Home"); // Return 404 if no program with the given ProgramName is found
            }

            return View(program); // Pass the retrieved Program object to the view
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Models.Program program)
        {
            if (ModelState.IsValid)
            {
                // Save changes to the database
                _context.Update(program);
                _context.SaveChanges();
                return RedirectToAction("EditProgram", "Admin");
            }
            return View(program);
        }

    }

}