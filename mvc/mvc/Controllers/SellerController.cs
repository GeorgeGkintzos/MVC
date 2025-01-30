using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using mvc.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace mvc.Controllers;
public class SellerController : Controller
{
    private readonly Final_Project_DBcontext _context;
    public SellerController(Final_Project_DBcontext context) // Κατασκευαστής
    {
        _context = context;
    }
    // Μέθοδος  για την εμφάνιση της αρχικής σελίδας του Seller
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult ClientList()// Μέθοδος για την εμφάνιση των πελατών
    {
        List<Models.User> users = _context.Users.Where(u => u.Property == "Client").ToList();
        return View(users);
    }

    public IActionResult RemoveFromClientsList(int id)// Μέθοδος για την αφαίρεση πελατών
    {
        var clientToRemove = _context.Clients.FirstOrDefault(s => s.UserId == id);
        if (clientToRemove != null)
        {
            _context.Clients.Remove(clientToRemove);
            var phoneToRemove = _context.Phones.FirstOrDefault(p => p.PhoneNumber == clientToRemove.PhoneNumber);
            if (phoneToRemove != null)
            {
                _context.Phones.Remove(phoneToRemove);
            }
            // Αλλαγή του Property του χρήστη σε "User"
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user != null)
            {
                user.Property = "User";
            }
            _context.SaveChanges();
        }
        else
        {
            return NotFound();
        }

        return RedirectToAction("ClientList", "Seller");
    }

    public IActionResult MakeClient(int id, string phoneNumber)// Μέθοδος για την δημιουργία πελατών
    {
        // Βρες τον χρήστη με το συγκεκριμένο id
        Models.User user = _context.Users.FirstOrDefault(u => u.UserId == id);
        if (user != null)
        {
            var existingPhone = _context.Phones.FirstOrDefault(p => p.PhoneNumber.Equals(phoneNumber));
            if (existingPhone == null)
            {
                var newPhone = new Models.Phone
                {
                    PhoneNumber = id  
                };
                _context.Phones.Add(newPhone);
            }
            Models.Client newClient = new Models.Client
            {
                UserId = id,
                PhoneNumber = id 
            };
            user.Property = "Client";
            _context.Clients.Add(newClient);
            _context.SaveChanges();
        }

        return RedirectToAction("CreateClient", "Seller");
    }
    public IActionResult CreateClient()// Μέθοδος για την δημιουργία πελατών
    {
        List<Models.User> users = _context.Users.Where(u => u.Property == "User").ToList();
        return View(users);
    }
    // Μέθοδος για την δημιουργία προγράμματος 
    public IActionResult ViewBills(int clientId)       
    {
        var bills = _context.Bills
            .Where(b => b.Bill_Id == clientId)
            .ToList();

        return View(bills);
    }

    public IActionResult PhoneList()// Μέθοδος για την εμφάνιση της λίστας των τηλεφώνων
    {
        var phones = _context.Phones.Include(p => p.ProgramNameNavigation).ToList();
        var programs = _context.Programs.ToList();
        ViewData["Programs"] = programs;
        return View(phones);
    }

    [HttpGet]
    public IActionResult UpdateProgram()// Μέθοδος για την ενημέρωση του προγράμματος
    {
        return View();
    }

    [HttpPost]
    public IActionResult UpdateProgram(IFormCollection form)// Μέθοδος για την ενημέρωση του προγράμματος
    {
        foreach (var key in form.Keys)
        {
            if (key.StartsWith("programName_"))
            {
                var phoneNumber = key.Replace("programName_", "");
                var programName = form[key];

                if (int.TryParse(phoneNumber, out int parsedPhoneNumber))
                {
                    var phone = _context.Phones.FirstOrDefault(p => p.PhoneNumber == parsedPhoneNumber);
                    if (phone != null)
                    {
                        phone.ProgramName = programName;
                        _context.SaveChanges();
                    }
                }
            }
        }

        return RedirectToAction("PhoneList");
    }

    public IActionResult Bills()// Μέθοδος για την εμφάνιση των λογαριασμών
    {
        var phones = _context.Phones.Include(p => p.ProgramNameNavigation).ToList();
        var programs = _context.Programs.ToList();
        ViewData["Programs"] = programs;
        return View(phones);
    }
    public IActionResult AddBill(int phoneNumber)// Μέθοδος για την προσθήκη λογαριασμού
    {
        var phone = _context.Phones.Include(p => p.ProgramNameNavigation).FirstOrDefault(p => p.PhoneNumber == phoneNumber);
        if (phone != null)
        {
            var client = _context.Clients.FirstOrDefault(c => c.PhoneNumber == phoneNumber);
            if (client != null)
            {
                int clientId = client.ClientId;
                int dateAsInt = int.Parse(DateTime.Now.ToString("MMddHHmmss"));
                int billId = clientId * 100000000 + dateAsInt; 
                Bill newBill = new Bill
                {
                    Bill_Id = billId,
                    PhoneNumber = phoneNumber,
                    Costs = phone.ProgramNameNavigation?.Charge, 
                    Paid = false
                };
                _context.Bills.Add(newBill);
                _context.SaveChanges();
            }

        }
        return RedirectToAction("Bills");
    }


}