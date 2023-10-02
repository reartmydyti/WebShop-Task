using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebshopService.Data;
using WebshopService.Models;

namespace WebShop.Controllers.MVC
{
    public class ProductsMVCController : Controller
    {
        private readonly WebshopContext _context;

        public ProductsMVCController(WebshopContext context)
        {
            _context = context;
        }

        // INDEX: List all products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);  // This expects a view named "Index" under Views/ProductsMVC
        }

        // DETAILS: Display a single product
        public async Task<IActionResult> Details(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);  // This expects a view named "Details" under Views/ProductsMVC
        }

        // CREATE GET: Display product creation form
        public IActionResult Create()
        {
            return View();  // This expects a view named "Create" under Views/ProductsMVC
        }

        // CREATE POST: Handle product creation form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // EDIT GET: Display product edit form
        public async Task<IActionResult> Edit(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);  // This expects a view named "Edit" under Views/ProductsMVC
        }

        // EDIT POST: Handle product edit form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            return View(product);
        }

        // DELETE GET: Confirm product deletion
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);  // This expects a view named "Delete" under Views/ProductsMVC
        }

        // DELETE POST: Handle product deletion confirmation
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(Guid id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
