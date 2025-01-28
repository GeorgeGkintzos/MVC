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

    public SellerController(Final_Project_DBcontext context)
    {
        _context = context;
    }

    // Προβολή πελατών που έχουν ανατεθεί στον πωλητή
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult ClientList()
    {
        List<Models.User> users = _context.Users.Where(u => u.Property == "Client").ToList();
        return View(users);
    }

    public IActionResult RemoveFromClientsList(int id)
    {
        // Βρες τον πελάτη από το UserId
        var clientToRemove = _context.Clients.FirstOrDefault(s => s.UserId == id);
        if (clientToRemove != null)
        {
            // Αφαίρεση του πελάτη από τον πίνακα Clients
            _context.Clients.Remove(clientToRemove);

            // Διαγραφή του τηλεφώνου από τον πίνακα Phones με βάση το PhoneNumber του πελάτη
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

            // Αποθήκευση των αλλαγών στη βάση δεδομένων
            _context.SaveChanges();
        }
        else
        {
            // Αν δεν βρέθηκε ο πελάτης, επιστροφή σφάλματος
            return NotFound();
        }

        return RedirectToAction("ClientList", "Seller");
    }

    public IActionResult MakeClient(int id, string phoneNumber)
    {
        // Βρες τον χρήστη με το συγκεκριμένο id
        Models.User user = _context.Users.FirstOrDefault(u => u.UserId == id);

        if (user != null)
        {
            // Έλεγχος αν το PhoneNumber υπάρχει ήδη στον πίνακα phones
            var existingPhone = _context.Phones.FirstOrDefault(p => p.PhoneNumber.Equals(phoneNumber));
            if (existingPhone == null)
            {
                // Αν δεν υπάρχει, πρόσθεσέ το στον πίνακα phones
                var newPhone = new Models.Phone
                {
                    PhoneNumber = id  // Χρησιμοποίησε το phoneNumber που παρέχεται
                };
                _context.Phones.Add(newPhone);
            }

            // Δημιουργία του νέου πελάτη
            Models.Client newClient = new Models.Client
            {
                UserId = id,
                PhoneNumber = id // Αποθηκεύεται και συνδέεται με τον πίνακα Phones
            };

            user.Property = "Client"; // Ενημέρωση του πεδίου Property στον πίνακα Users

            _context.Clients.Add(newClient);
            _context.SaveChanges();
        }

        return RedirectToAction("CreateClient", "Seller");
    }

    // Δημιουργία νέου πελάτη
    public IActionResult CreateClient()
    {
        List<Models.User> users = _context.Users.Where(u => u.Property == "User").ToList();
        return View(users); // Pass List<Models.User> to the view
    }

    // Προβολή λογαριασμών πελατών
    public IActionResult ViewBills(int clientId)
    {
        var bills = _context.Bills
            .Where(b => b.Bill_Id == clientId)
            .ToList();

        return View(bills);
    }

    public IActionResult PhoneList()
    {
        var phones = _context.Phones.Include(p => p.ProgramNameNavigation).ToList();
        var programs = _context.Programs.ToList();

        // Διασφαλίζουμε ότι τα προγράμματα αποδίδονται στη ViewData
        ViewData["Programs"] = programs;

        return View(phones);
    }

    [HttpGet]
    public IActionResult UpdateProgram()
    {
        // Your GET logic here
        return View();
    }

    [HttpPost]
    public IActionResult UpdateProgram(IFormCollection form)
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

    public IActionResult Bills()
    {
        var phones = _context.Phones.Include(p => p.ProgramNameNavigation).ToList();
        var programs = _context.Programs.ToList();

        // Διασφαλίζουμε ότι τα προγράμματα αποδίδονται στη ViewData
        ViewData["Programs"] = programs;

        return View(phones);
    }

    public IActionResult AddBill(int phoneNumber)
    {
        // Find the phone with the specific PhoneNumber
        var phone = _context.Phones.Include(p => p.ProgramNameNavigation).FirstOrDefault(p => p.PhoneNumber == phoneNumber);

        if (phone != null)
        {
            var client = _context.Clients.FirstOrDefault(c => c.PhoneNumber == phoneNumber);

            if (client != null)
            {
                int clientId = client.ClientId;

                // Shortened timestamp format (MMddHHmm)
                int dateAsInt = int.Parse(DateTime.Now.ToString("MMddHHmmss"));

                // Combine ClientId and dateAsInt in a way that fits within an int
                int billId = clientId * 100000000 + dateAsInt; // Multiply to avoid overlap

                // Create the new bill
                Bill newBill = new Bill
                {
                    Bill_Id = billId, // Set the combined value as BillId
                    PhoneNumber = phoneNumber,
                    Costs = phone.ProgramNameNavigation?.Charge, // Use the Charge from the program
                    Paid = false // Default to unpaid
                };

                // Add the new bill to the database
                _context.Bills.Add(newBill);
                _context.SaveChanges();
            }

        }

        return RedirectToAction("Bills");
    }


}