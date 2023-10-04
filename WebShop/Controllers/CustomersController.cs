using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShop.Models;
using WebshopService.Data;
using WebshopService.Models;

namespace WebshopService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly WebshopContext _context;

        public CustomersController(WebshopContext context)
        {
            _context = context;
        }

        // CREATE: New customer
        [HttpPost]
        public async Task<IActionResult> CreateCustomer(Customer customer)
        {
            if (customer == null)
                return BadRequest("Invalid customer data.");

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.CustomerId }, customer);
        }

        // READ: Get all customers
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _context.Customers
                                   .Include(c => c.Address)
                                   .Include(c => c.Orders)
                                   .ToListAsync();

            var result = customers.Select(c => new
            {
                c.CustomerId,
                c.Name,
                c.Firstname,
                c.EmailAddress,
                c.Address,
                c.Birthdate,
                c.JobTitle,
                c.Category,
                Orders = c.Orders.Any() ? c.Orders : new List<Order> { new Order { AdditionalInfo = "No Orders" } }
            });

            return Ok(result);
        }


        // READ: Get a single customer by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            var customer = await _context.Customers.Include(c => c.Address).FirstOrDefaultAsync(c => c.CustomerId == id);
            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        // UPDATE: Update a customer
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(Guid id, Customer updatedCustomer)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            customer.Name = updatedCustomer.Name;
            customer.Firstname = updatedCustomer.Firstname;
            customer.EmailAddress = updatedCustomer.EmailAddress;
            customer.Address = updatedCustomer.Address;
            customer.Birthdate = updatedCustomer.Birthdate;
            customer.JobTitle = updatedCustomer.JobTitle;
            customer.Category = updatedCustomer.Category;
            

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: Delete a customer
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // NEW: Filter customers by name, firstname, email, and category
        [HttpGet("filter")]
        public async Task<IActionResult> FilterCustomers(string? name = null, string? firstname = null, string? email = null, string? category = null)
        {
            var query = _context.Customers.Include(c => c.Address).AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(c => c.Name.Contains(name));

            if (!string.IsNullOrEmpty(firstname))
                query = query.Where(c => c.Firstname.Contains(firstname));

            if (!string.IsNullOrEmpty(email))
                query = query.Where(c => c.EmailAddress.Contains(email));

            if (!string.IsNullOrEmpty(category))
                query = query.Where(c => c.Category.ToString() == category);

            var result = await query.ToListAsync();

            return Ok(result);
        }

        // NEW: Get customers by region
        [HttpGet("region")]
        public async Task<IActionResult> GetCustomersByRegion(string? city = null, string? country = null, string? state = null)
        {
            var query = _context.Customers.Include(c => c.Address).AsQueryable();

            if (!string.IsNullOrEmpty(city))
                query = query.Where(c => c.Address.City == city);

            if (!string.IsNullOrEmpty(country))
                query = query.Where(c => c.Address.Country == country);

            if (!string.IsNullOrEmpty(state))
                query = query.Where(c => c.Address.State == state);

            var result = await query.ToListAsync();

            return Ok(result);
        }


        // NEW: Get customers by product ID in their orders
        [HttpGet("byproduct/{productId}")]
        public async Task<IActionResult> GetCustomersByProduct(Guid productId)
        {
            var customerIds = _context.Orders.Where(o => o.Products.Any(p => p.ProductId == productId)).Select(o => o.CustomerId).Distinct();
            var customers = await _context.Customers.Where(c => customerIds.Contains(c.CustomerId)).ToListAsync();
            return Ok(customers);
        }
    }
}
