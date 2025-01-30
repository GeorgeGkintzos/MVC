using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using mvc.Models;

namespace mvc.Controllers;
public class ClientController : Controller
{
    private readonly Final_Project_DBcontext _context;

    public ClientController(Final_Project_DBcontext context)
    {
        _context = context;
    }

    public IActionResult Index()// Μέθοδος Index για την εμφάνιση της αρχικής σελίδας του Client
    {
        return View();
    }

    public IActionResult EditClient()// Μέθοδος για την εμφάνιση της φόρμας επεξεργασίας του πελάτη
    {
        return View();
    }
    {
        // Ανάκτηση UserId από τη συνεδρία
        int? userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToAction("Login", "Account"); // Ανακατεύθυνση στη σελίδα login αν το UserId δεν υπάρχει
        }

        // Ανάκτηση του πελάτη με βάση το UserId
        var client = _context.Clients.FirstOrDefault(c => c.UserId == userId.Value);

        if (client == null)
        {
            return NotFound();
        }

        return View(client); 
    }

    [HttpPost]
    public IActionResult Edit(Client model)// Μέθοδος για την ενημέρωση των στοιχείων του πελάτη
{
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }
        var client = _context.Clients.FirstOrDefault(c => c.UserId == userId.Value);
        if (client == null)
        {
            return NotFound();
        }
        // Αποθήκευση του ProgramName πριν την ενημέρωση του PhoneNumber
        string? previousProgramName = null;
        if (model.PhoneNumber.HasValue)
        {
            var phoneToUpdate = _context.Phones.FirstOrDefault(p => p.PhoneNumber == client.PhoneNumber);
            if (phoneToUpdate != null)
            {
                previousProgramName = phoneToUpdate.ProgramName;
            }
        }

        // Έλεγχος αν το PhoneNumber υπάρχει στον πίνακα Phones
        var existingPhone = _context.Phones.FirstOrDefault(p => p.PhoneNumber == model.PhoneNumber);

        if (existingPhone == null && model.PhoneNumber.HasValue) // Ελέγχουμε αν το PhoneNumber έχει τιμή
        {
            Console.WriteLine("Το νέο PhoneNumber δεν υπάρχει, το προσθέτουμε.");
            existingPhone = new Phone
            {
                PhoneNumber = model.PhoneNumber.Value,
                ProgramName = previousProgramName
            };
            _context.Phones.Add(existingPhone);
        }
        else if (existingPhone != null && model.PhoneNumber.HasValue)
        {
            Console.WriteLine("Το PhoneNumber υπάρχει ήδη, ενημερώνουμε.");
            existingPhone.PhoneNumber = model.PhoneNumber.Value;
            existingPhone.ProgramName = previousProgramName;
        }
        // Ενημέρωση του PhoneNumber στους Bills
        if (model.PhoneNumber.HasValue)
        {
            Console.WriteLine("Ενημέρωση PhoneNumber στους Bills...");
            var billsToUpdate = _context.Bills.Where(b => b.PhoneNumber == client.PhoneNumber).ToList();
            foreach (var bill in billsToUpdate)
            {
                bill.PhoneNumber = model.PhoneNumber.Value;// Ενημέρωση του PhoneNumber στον πίνακα Bills
            }
        }

        // Έλεγχος ότι οι κλήσεις με το παλιό PhoneNumber ενημερώνονται σωστά
        var callsToUpdate = _context.Calls.Where(c => c.PhoneNumber == client.PhoneNumber).ToList();

        if (callsToUpdate.Any())
        {
            Console.WriteLine($"Βρέθηκαν {callsToUpdate.Count} κλήσεις με το PhoneNumber {client.PhoneNumber}");

            foreach (var call in callsToUpdate)
            {
                Console.WriteLine($"Ενημέρωση Call με ID {call.CallId}, Παλαιό PhoneNumber: {call.PhoneNumber}, Νέο PhoneNumber: {model.PhoneNumber}");
                call.PhoneNumber = model.PhoneNumber.Value; 
            }

            _context.SaveChanges();
            Console.WriteLine("Οι αλλαγές στους Calls αποθηκεύτηκαν επιτυχώς.");
        }
        else
        {
            Console.WriteLine("Δεν βρέθηκαν κλήσεις με το PhoneNumber.");
        }

        // Ενημέρωση του πελάτη με το νέο PhoneNumber
        if (model.PhoneNumber.HasValue)
        {
            client.Afm = model.Afm;
            client.PhoneNumber = model.PhoneNumber.Value;
        }

        // Αποθήκευση όλων των αλλαγών
        _context.SaveChanges();

        // Διαγραφή όλων των PhoneNumber που δεν ταυτίζονται με κανένα Client
        var phonesToDelete = _context.Phones
            .Where(p => !_context.Clients.Any(c => c.PhoneNumber == p.PhoneNumber))
            .ToList();

        _context.Phones.RemoveRange(phonesToDelete);
        _context.SaveChanges();

        ViewData["Message"] = "Τα στοιχεία ενημερώθηκαν με επιτυχία!";

        return View(client);
    }



    public IActionResult ViewProgram()// Μέθοδος για την εμφάνιση του προγράμματος του Client
{
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }
        var client = _context.Clients.FirstOrDefault(c => c.UserId == userId.Value);
        if (client == null)
        {
            return NotFound("Ο πελάτης δεν βρέθηκε.");
        }
        string phoneNumber = client.PhoneNumber.ToString();
        // Αναζήτηση του PhoneNumber και του Program_Name 
        var phoneDetails = _context.Phones
                                   .Where(p => p.PhoneNumber.ToString() == phoneNumber.ToString())
                                   .Select(p => new
                                   {
                                       PhoneNumber = p.PhoneNumber,
                                       ProgramName = p.ProgramName
                                   })
                                   .FirstOrDefault();
        if (phoneDetails == null)
        {
            return NotFound("Δεν βρέθηκαν λεπτομέρειες για τον αριθμό τηλεφώνου.");
        }
        // Επιστροφή των δεδομένων στο View
        ViewBag.PhoneNumber = phoneDetails.PhoneNumber;
        ViewBag.ProgramName = phoneDetails.ProgramName;

        return View();
    }

    public IActionResult ViewBills()// Μέθοδος για την εμφάνιση των λογαριασμών του Client
{
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }
        var client = _context.Clients.FirstOrDefault(c => c.UserId == userId.Value);

        if (client == null)
        {
            return NotFound("Client not found.");
        }

        string phoneNumber = client.PhoneNumber.ToString();
    // Αναζήτηση όλων των λογαριασμον για τον συγκεκριμένο αριθμό τηλεφώνου
    var bills = _context.Bills
                            .Where(b => b.PhoneNumber == client.PhoneNumber)
                            .ToList();

        var unpaidBills = bills.Where(b => b.Paid == false).ToList();
        var paidBills = bills.Where(b => b.Paid == true).ToList();

        // Pass data to the view
        ViewData["UnpaidBills"] = unpaidBills;
        ViewData["PaidBills"] = paidBills;

        return View();
    }

    [HttpPost]
    public IActionResult MarkAsPaid(int billId)// Μέθοδος για την ενημέρωση του λογαριασμού ως πληρωμένο
{
        var bill = _context.Bills.FirstOrDefault(b => b.Bill_Id == billId);
        if (bill != null)
        {
            bill.Paid = true;
            _context.SaveChanges();
        }
        return RedirectToAction("ViewBills");
    }

    public IActionResult ViewCalls()// Μέθοδος για την εμφάνιση των κλήσεων του Client
{
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }
        var client = _context.Clients.FirstOrDefault(c => c.UserId == userId.Value);
        if (client == null)
        {
            return NotFound("Ο πελάτης δεν βρέθηκε.");
        }
    // Αναζήτηση όλων των κλήσεων για τον συγκεκριμένο αριθμό τηλεφώνου
    var calls = _context.Calls
            .Where(c => c.PhoneNumber == client.PhoneNumber)
            .Select(c => new
            {
                CallId = c.CallId,
                PhoneNumber = c.PhoneNumber,
                Incoming = c.Incoming
            })
            .ToList();
        ViewData["Calls"] = calls;

        return View(calls);
    }
}
