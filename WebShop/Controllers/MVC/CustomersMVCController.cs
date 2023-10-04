using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebshopService.Data;
using WebshopService.Models;

namespace WebShop.Controllers.MVC
{
    public class CustomersMVCController : Controller
    {
        private readonly WebshopContext _context;

        public CustomersMVCController(WebshopContext context)
        {
            _context = context;
        }

        // INDEX: List all customers
        public async Task<IActionResult> Index()
        {
            var customers = await _context.Customers
                .Include(c => c.Address)
                .Include(c => c.Orders)
                .ToListAsync();
            return View(customers);
        }

        // DETAILS: Display a single customer
        public async Task<IActionResult> Details(Guid id)
        {
            var customer = await _context.Customers.Include(c => c.Address).FirstOrDefaultAsync(c => c.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // CREATE GET: Display customer creation form
        public IActionResult Create()
        {
            return View();
        }

        // CREATE POST: Handle customer creation form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            Console.WriteLine($"Received Category: {customer.Category}");
            if (!ModelState.IsValid)
            {
                // Log or handle the validation errors here
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage); // logging the errors to the console
                    }
                }
                return View(customer);  // Return to the view to show errors and correct them
            }

            if (customer.Address == null)
            {
                customer.Address = new CustomerAddress();
            }
            _context.Add(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // EDIT GET: Display customer edit form
        public async Task<IActionResult> Edit(Guid id)
        {
            var customer = await _context.Customers
                                .Include(c => c.Address) // Eager load the Address
                                .FirstOrDefaultAsync(c => c.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // EDIT POST: Handle customer edit form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // DELETE GET: Confirm customer deletion
        public async Task<IActionResult> Delete(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // DELETE POST: Handle customer deletion confirmation
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(Guid id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }

        public IActionResult FilterCustomers()
        {
            return View();
        }

        public IActionResult GetCustomersByRegion()
        {
            return View();
        }

        public IActionResult GetCustomersByProduct()
        {
            return View();
        }

    }
}
