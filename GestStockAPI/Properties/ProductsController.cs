using Microsoft.AspNetCore.Mvc;
using GestStockAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GestStockAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
public async Task<IActionResult> GetProducts(bool simplified = false)
{
    
    var productsQuery = _context.Products
                                .Include(p => p.Category)
                                .Where(p => !p.IsDeleted);

    if (simplified)
    {
       
        var simplifiedProducts = await productsQuery
            .Select(p => new
            {
                p.ProductId,
                p.ProductName,
                p.Category.CatName
            })
            .ToListAsync();

        return Ok(simplifiedProducts);
    }

  
    var products = await productsQuery
        .Select(p => new ProductRequest
        {
            ProductId = p.ProductId,  
            ProductName = p.ProductName,
            ProductDescription = p.ProductDescription,
            Price = p.Price,
            CatName = p.Category.CatName
        })
        .ToListAsync();

    return Ok(products);
}




        [HttpPut("ModifyProduct")]
public async Task<IActionResult> ModifyProduct([FromBody] ProductRequest request)
{
    var product = await _context.Products.FindAsync(request.ProductId);
    if (product == null)
    {
        return NotFound("Product not found");
    }

    product.ProductName = request.ProductName;
    product.ProductDescription = request.ProductDescription;
    product.Price = request.Price;

    // Update the category based on the CatName if necessary
    var category = await _context.Categories.FirstOrDefaultAsync(c => c.CatName == request.CatName);
    if (category != null)
    {
        product.CategoryId = category.CategoryId;
    }

    await _context.SaveChangesAsync();

    return Ok("Product modified successfully");
}


        [HttpGet("{id}")]
        public async Task<ActionResult<ProductRequest>> GetAllProducts(int id)
        {
            var product = await _context.Products
                                        .Include(p => p.Category)
                                        .FirstOrDefaultAsync(p => p.ProductId == id && !p.IsDeleted);

            if (product == null)
            {
                return NotFound();
            }

            return new ProductRequest
            {
                
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                Price = product.Price,
                CatName = product.Category.CatName,
            };
        }


        [HttpPost]
        public async Task<ActionResult<ProductRequest>> PostProduct(ProductRequest productRequest)
        {

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CatName == productRequest.CatName);
            if (category == null)
            {
                return BadRequest("Invalid category name.");
            }

            var product = new Product
            {
                CategoryId = category.CategoryId,
                ProductName = productRequest.ProductName,
                ProductDescription = productRequest.ProductDescription,
                Price = productRequest.Price,
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProducts), new { id = product.ProductId }, productRequest);
        }

        [HttpPut("UpdateProduct")]
public async Task<IActionResult> UpdateProduct([FromBody] ProductRequest productRequest)
{
   
    var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productRequest.ProductId);
    
    if (product == null)
    {
        return NotFound("Product not found.");
    }

    
    product.ProductName = productRequest.ProductName;
    product.ProductDescription = productRequest.ProductDescription;
    product.Price = productRequest.Price;

    
    var category = await _context.Categories.FirstOrDefaultAsync(c => c.CatName == productRequest.CatName);
    
    if (category != null)
    {
        product.CategoryId = category.CategoryId;
    }
    else
    {
        return NotFound("Category not found.");
    }
    await _context.SaveChangesAsync();

    return NoContent();
}


    
        private bool ProductExists(int id)
        {
            return _context.Products.Any(p => p.ProductId == id);
        }

        [HttpGet("Getonly")]
public async Task<IActionResult> GetProductList(string categoryName = null)
{
    var productsQuery = _context.Products.AsQueryable();

    if (!string.IsNullOrEmpty(categoryName))
    {
        productsQuery = productsQuery.Where(p => p.Category.CatName == categoryName);
    }

    var products = await productsQuery
        .Select(p => new { p.ProductId, p.ProductName, p.Category.CatName })
        .ToListAsync();

    return Ok(products);
}

   [HttpPut("SoftDeleteProduct/{id}")]
public async Task<IActionResult> SoftDeleteProduct(int id)
{
    var product = await _context.Products.FindAsync(id);
    if (product == null)
    {
        return NotFound("Product not found");
    }

    product.IsDeleted = true;
    await _context.SaveChangesAsync();

    return Ok("Product deleted successfully");
}


    }
}
