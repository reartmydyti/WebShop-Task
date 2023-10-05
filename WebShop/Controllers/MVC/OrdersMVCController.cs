using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Security.Claims;
using WebShop.Models;
using WebshopService.Data;
using WebshopService.Models;

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
            var orders = await _context.Orders
                .Include(o => o.Products)
                .Include(o => o.Customer)
                .ToListAsync();
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
            ViewBag.AllProducts = _context.Products.ToList();
            ViewBag.Customers = _context.Customers.ToList();  // Fetching the customers list
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order, List<Guid> selectedProducts)
        {
            if (ModelState.IsValid)
            {
                var customer = await _context.Customers.FindAsync(order.CustomerId);
                if (customer == null)
                {
                    ModelState.AddModelError("CustomerId", "Invalid Customer ID.");
                    ViewBag.AllProducts = _context.Products.ToList();
                    ViewBag.Customers = _context.Customers.ToList();  // Fetching the customers list again for the view
                    return View(order);
                }

                order.Customer = customer;

                if (selectedProducts != null)
                {
                    order.Products = _context.Products.Where(p => selectedProducts.Contains(p.ProductId)).ToList();
                }

                _context.Add(order);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.AllProducts = _context.Products.ToList();
            ViewBag.Customers = _context.Customers.ToList();  // Fetching the customers list again for the view in case of a model error
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

            ViewBag.AllProducts = await _context.Products.ToListAsync();
            ViewBag.Customers = await _context.Customers.ToListAsync();

            return View(order);
        }

        // EDIT POST: Handle order edit form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Order model, List<Guid> selectedProducts)
        {
            if (id != model.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var orderToUpdate = await _context.Orders.Include(o => o.Products).FirstOrDefaultAsync(o => o.OrderId == id);
                    if (orderToUpdate == null)
                        return NotFound();

                    // Update simple properties
                    orderToUpdate.CustomerId = model.CustomerId;
                    orderToUpdate.AdditionalInfo = model.AdditionalInfo;

                    // Handle products. Remove all existing relationships and then add the new ones
                    orderToUpdate.Products.Clear();
                    var productsToAdd = _context.Products.Where(p => selectedProducts.Contains(p.ProductId)).ToList();
                    foreach (var product in productsToAdd)
                    {
                        orderToUpdate.Products.Add(product);
                    }

                    _context.Update(orderToUpdate);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(model.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(model);
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

        public IActionResult GetOrdersByCustomerId()
        {
            return View();
        }

        public IActionResult GetOrdersByNewCustomers()
        {
            return View();
        }


        [HttpGet("SearchByCustomer")]
        public IActionResult SearchByCustomer()
        {
            return View();
        }



    }
}