using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebshopService.Data;
using WebshopService.Models;

namespace WebshopService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly WebshopContext _context;

        public ProductsController(WebshopContext context)
        {
            _context = context;
        }

        // CREATE: New product
        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            if (product == null)
                return BadRequest("Invalid product data.");

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, product);
        }

        // READ: Get all products
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        // READ: Get a single product by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // UPDATE: Update a product
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, Product updatedProduct)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            product.Label = updatedProduct.Label;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: Delete a product
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
