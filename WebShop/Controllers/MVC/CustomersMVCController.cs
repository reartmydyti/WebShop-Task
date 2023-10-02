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
            var customers = await _context.Customers.Include(c => c.Address).ToListAsync();
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
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // EDIT GET: Display customer edit form
        public async Task<IActionResult> Edit(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);
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
    }
}
