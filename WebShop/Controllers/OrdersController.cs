using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShop.Models;
using WebshopService.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebshopService.Models;

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
            var orders = await _context.Orders
                .Include(o => o.Products)
                .Include(o => o.Customer)
                .ToListAsync();
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

        // Get orders for a specific customer
        [HttpGet("GetOrdersByCustomerId")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByCustomerId(Guid? customerId)
        {
            if (!customerId.HasValue)
            {
                return BadRequest("Customer ID is required.");
            }

            var orders = await _context.Orders
                                       .Include(o => o.Customer)  // This line includes the related Customer data
                                       .Include(o => o.Products)
                                       .Where(o => o.CustomerId == customerId.Value)
                                       .ToListAsync();

            if (!orders.Any())
            {
                return NotFound($"No orders found for Customer ID: {customerId.Value}");
            }

            return Ok(orders);
        }




        // NEW: Get orders placed by customers with the state NEW_CUSTOMER
        [HttpGet("newcustomers")]
        public async Task<IActionResult> GetOrdersByNewCustomers()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.Customer.Category == CustomerCategory.NEW_CUSTOMER)
                .ToListAsync();

            return Ok(orders);
        }




    }
}
