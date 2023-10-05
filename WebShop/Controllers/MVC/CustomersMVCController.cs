using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebshopService.Data;
using WebshopService.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebShop.Controllers.MVC
{
    public class CustomersMVCController : Controller
    {
        private readonly WebshopContext _context;

        public CustomersMVCController(WebshopContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Customers.Include(c => c.Address).Include(c => c.Orders).ToListAsync());
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var customer = await FindCustomerByIdAsync(id);
            return customer == null ? NotFound() : View(customer);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            customer.Address ??= new CustomerAddress();
            _context.Add(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var customer = await FindCustomerByIdAsync(id);
            return customer == null ? NotFound() : View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            _context.Update(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var customer = await FindCustomerByIdAsync(id);
            return customer == null ? NotFound() : View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var customer = await FindCustomerByIdAsync(id);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(Guid id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }

        private Task<Customer> FindCustomerByIdAsync(Guid id)
        {
            return _context.Customers.Include(c => c.Address).FirstOrDefaultAsync(c => c.CustomerId == id);
        }

        public IActionResult FilterCustomers() => View();
        public IActionResult GetCustomersByRegion() => View();
        public IActionResult GetCustomersByProduct() => View();
    }
}
