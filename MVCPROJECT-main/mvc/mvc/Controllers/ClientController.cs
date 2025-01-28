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

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult EditClient()
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
            return NotFound(); // Επιστροφή 404 αν ο πελάτης δεν βρεθεί
        }

        return View(client); // Εμφάνιση της θέας με τον συγκεκριμένο πελάτη
    }

    [HttpPost]
    public IActionResult Edit(Client model)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Βρες τον πελάτη με το συγκεκριμένο UserId
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
                previousProgramName = phoneToUpdate.ProgramName;  // Αποθήκευση του προηγούμενου ProgramName
            }
        }

        // Έλεγχος αν το PhoneNumber υπάρχει στον πίνακα Phones
        var existingPhone = _context.Phones.FirstOrDefault(p => p.PhoneNumber == model.PhoneNumber);

        if (existingPhone == null && model.PhoneNumber.HasValue) // Ελέγχουμε αν το PhoneNumber έχει τιμή
        {
            Console.WriteLine("Το νέο PhoneNumber δεν υπάρχει, το προσθέτουμε.");
            existingPhone = new Phone
            {
                PhoneNumber = model.PhoneNumber.Value, // Χρησιμοποιούμε το PhoneNumber από το μοντέλο
                ProgramName = previousProgramName // Επαναφορά του ProgramName
            };
            _context.Phones.Add(existingPhone);
        }
        else if (existingPhone != null && model.PhoneNumber.HasValue)
        {
            Console.WriteLine("Το PhoneNumber υπάρχει ήδη, ενημερώνουμε.");
            // Αν το PhoneNumber υπάρχει ήδη, διατηρούμε το ProgramName και ενημερώνουμε μόνο το PhoneNumber
            existingPhone.PhoneNumber = model.PhoneNumber.Value; // Ενημέρωση του PhoneNumber
            existingPhone.ProgramName = previousProgramName; // Διατηρούμε το ProgramName
        }

        // Ενημέρωση του PhoneNumber στους Bills
        if (model.PhoneNumber.HasValue)
        {
            Console.WriteLine("Ενημέρωση PhoneNumber στους Bills...");
            var billsToUpdate = _context.Bills.Where(b => b.PhoneNumber == client.PhoneNumber).ToList();
            foreach (var bill in billsToUpdate)
            {
                bill.PhoneNumber = model.PhoneNumber.Value; // Ενημέρωση του PhoneNumber στον πίνακα Bills
            }
        }

        // Βεβαιωθείτε ότι οι κλήσεις με το παλιό PhoneNumber ενημερώνονται σωστά
        var callsToUpdate = _context.Calls.Where(c => c.PhoneNumber == client.PhoneNumber).ToList();

        if (callsToUpdate.Any())
        {
            Console.WriteLine($"Βρέθηκαν {callsToUpdate.Count} κλήσεις με το PhoneNumber {client.PhoneNumber}");

            foreach (var call in callsToUpdate)
            {
                Console.WriteLine($"Ενημέρωση Call με ID {call.CallId}, Παλαιό PhoneNumber: {call.PhoneNumber}, Νέο PhoneNumber: {model.PhoneNumber}");
                call.PhoneNumber = model.PhoneNumber.Value; // Ενημέρωση του PhoneNumber
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



    public IActionResult ViewProgram()
    {
        // Ανάκτηση του PhoneNumber του Client από τη συνεδρία ή άλλο σημείο
        int? userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Ανάκτηση του PhoneNumber από τον πίνακα Clients
        var client = _context.Clients.FirstOrDefault(c => c.UserId == userId.Value);

        if (client == null)
        {
            return NotFound("Ο πελάτης δεν βρέθηκε.");
        }

        string phoneNumber = client.PhoneNumber.ToString();

        // Αναζήτηση του PhoneNumber και του Program_Name στον πίνακα Phones
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

    public IActionResult ViewBills()
    {
        // Retrieve the UserId from the session
        int? userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToAction("Login", "Account"); // Redirect to login if UserId is not found
        }

        // Retrieve the client's phone number from the database
        var client = _context.Clients.FirstOrDefault(c => c.UserId == userId.Value);

        if (client == null)
        {
            return NotFound("Client not found.");
        }

        string phoneNumber = client.PhoneNumber.ToString();

        // Retrieve bills grouped by paid status
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
    public IActionResult MarkAsPaid(int billId)
    {
        // Retrieve the bill by its BillId
        var bill = _context.Bills.FirstOrDefault(b => b.Bill_Id == billId);

        if (bill != null)
        {
            bill.Paid = true; // Mark the bill as paid
            _context.SaveChanges(); // Save the changes to the database
        }

        return RedirectToAction("ViewBills"); // Redirect back to the list of bills
    }

    public IActionResult ViewCalls()
    {
        // Ανάκτηση του UserId από τη συνεδρία
        int? userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToAction("Login", "Account"); // Ανακατεύθυνση στη σελίδα login αν το UserId δεν υπάρχει
        }

        // Ανάκτηση του πελάτη με βάση το UserId
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
            .ToList(); // Get a list of anonymous objects

        // Περάστε τα δεδομένα στην θέα ως dynamic
        ViewData["Calls"] = calls;

        return View(calls);  // Pass the list of calls directly to the view
    }
}
