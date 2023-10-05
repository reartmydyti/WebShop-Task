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

        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var product = await FindProductById(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var product = await FindProductById(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Product product)
        {
            if (id != product.ProductId || !ModelState.IsValid)
                return NotFound();

            try
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.ProductId))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await FindProductById(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var product = await FindProductById(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<Product> FindProductById(Guid id) =>
            await _context.Products.FindAsync(id);

        private bool ProductExists(Guid id) =>
            _context.Products.Any(e => e.ProductId == id);
    }
}
