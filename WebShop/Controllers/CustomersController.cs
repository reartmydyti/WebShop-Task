using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShop.Models;
using WebshopService.Data;
using WebshopService.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(Customer customer)
        {
            if (customer == null)
                return BadRequest("Invalid customer data.");

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.CustomerId }, customer);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _context.Customers
                                   .Include(c => c.Address)
                                   .Include(c => c.Orders)
                                   .ToListAsync();

            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            var customer = await FindCustomerByIdAsync(id);
            return customer == null ? NotFound() : Ok(customer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(Guid id, Customer updatedCustomer)
        {
            var customer = await FindCustomerByIdAsync(id);
            if (customer == null)
                return NotFound();

            // Copy the properties you want to update
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var customer = await FindCustomerByIdAsync(id);
            if (customer == null)
                return NotFound();

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        //Filter operations to search for customers by name, firstname, email addresses, and category
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
            {
                if (Enum.TryParse(typeof(CustomerCategory), category, out var categoryValue))
                {
                    query = query.Where(c => c.Category == (CustomerCategory)categoryValue);
                }
                else
                {
                    return BadRequest("Invalid category value.");
                }
            }

            return Ok(await query.ToListAsync());
        }




        //Retrieve a list of customers living in the same region (city, country, state).
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

            return Ok(await query.ToListAsync());
        }

        //Retrieve a list of customers who have a specific product ID in their orders.
        [HttpGet("byproduct/{productId}")]
        public async Task<IActionResult> GetCustomersByProduct(Guid productId)
        {
            var customerIds = _context.Orders.Where(o => o.Products.Any(p => p.ProductId == productId)).Select(o => o.CustomerId).Distinct();
            var customers = await _context.Customers.Where(c => customerIds.Contains(c.CustomerId)).ToListAsync();
            return Ok(customers);
        }

        private Task<Customer> FindCustomerByIdAsync(Guid id)
        {
            return _context.Customers.Include(c => c.Address).FirstOrDefaultAsync(c => c.CustomerId == id);
        }
    }
}
