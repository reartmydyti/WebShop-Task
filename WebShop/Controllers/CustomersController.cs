using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var customers = await _context.Customers.Include(c => c.Address).ToListAsync();
            return Ok(customers);
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
    }
}
