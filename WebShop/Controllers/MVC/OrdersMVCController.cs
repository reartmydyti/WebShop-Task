using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShop.Models;
using WebshopService.Data;

namespace WebShop.Controllers.MVC
{
    public class OrdersMVCController : Controller
    {
        private readonly WebshopContext _context;

        public OrdersMVCController(WebshopContext context)
        {
            _context = context;
        }

        // INDEX: List all orders
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders.Include(o => o.Products).ToListAsync();
            return View(orders);
        }

        // DETAILS: Display a single order
        public async Task<IActionResult> Details(Guid id)
        {
            var order = await _context.Orders.Include(o => o.Products).FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // CREATE GET: Display order creation form
        public IActionResult Create()
        {
            return View();
        }

        // CREATE POST: Handle order creation form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // EDIT GET: Display order edit form
        public async Task<IActionResult> Edit(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // EDIT POST: Handle order edit form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            return View(order);
        }

        // DELETE GET: Confirm order deletion
        public async Task<IActionResult> Delete(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // DELETE POST: Handle order deletion confirmation
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(Guid id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
