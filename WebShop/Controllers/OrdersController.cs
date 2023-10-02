using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShop.Models;
using WebshopService.Data;

namespace WebshopService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly WebshopContext _context;

        public OrdersController(WebshopContext context)
        {
            _context = context;
        }

        // CREATE: New order
        [HttpPost]
        public async Task<IActionResult> CreateOrder(Order order)
        {
            if (order == null)
                return BadRequest("Invalid order data.");

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
        }

        // READ: Get all orders
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _context.Orders.Include(o => o.Products).ToListAsync();
            return Ok(orders);
        }

        // READ: Get a single order by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _context.Orders.Include(o => o.Products).FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // UPDATE: Update an order
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(Guid id, Order updatedOrder)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            order.AdditionalInfo = updatedOrder.AdditionalInfo;
            order.Products = updatedOrder.Products;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: Delete an order
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
