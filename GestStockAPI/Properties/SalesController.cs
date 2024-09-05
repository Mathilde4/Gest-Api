using Microsoft.AspNetCore.Mvc;
using GestStockAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GestStockAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleRequest>>> GetSales()
        {
            var sales = await _context.Sales.Include(s => s.Product).Include(s => s.Category).ToListAsync();

            return sales.Select(s => new SaleRequest
            {
                Quantity = s.Quantity,
                UnitPrice = s.UnitPrice,
                Date = s.Date,
                ProductName = s.Product.ProductName,
                CatName = s.Category.CatName,
                ClientName = s.ClientName
            }).ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SaleRequest>> GetSale(int id)
        {
            var sale = await _context.Sales.Include(s => s.Product).Include(s => s.Category).FirstOrDefaultAsync(s => s.SaleId == id);

            if (sale == null)
            {
                return NotFound();
            }

            return new SaleRequest
            {
                Quantity = sale.Quantity,
                UnitPrice = sale.UnitPrice,
                Date = sale.Date,
                ProductName = sale.Product.ProductName,
                CatName = sale.Category.CatName,
                ClientName = sale.ClientName,
                
            };
        }

        [HttpPost]
        public async Task<IActionResult> AddSale([FromBody] SaleRequest saleRequest)
        {
           
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductName == saleRequest.ProductName);
            if (product == null)
            {
                return BadRequest("Invalid product name.");
            }

            
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CatName == saleRequest.CatName);
            if (category == null)
            {
                return BadRequest("Invalid category name.");
            }

            
            var maxReceiptNumber = await _context.Sales.MaxAsync(s => (int?)s.ReceiptNumber) ?? 0;
            int newReceiptNumber = maxReceiptNumber + 1;

           
            var sale = new Sale
            {
                ProductId = product.ProductId,
                CategoryId = category.CategoryId,
                Quantity = saleRequest.Quantity,
                UnitPrice = saleRequest.UnitPrice,
                Date = DateTime.Now,
                ClientName = saleRequest.ClientName,
                ReceiptNumber = newReceiptNumber 
            };

            _context.Sales.Add(sale);

            var totalSales = await _context.TotalSales.FirstOrDefaultAsync();
            if (totalSales != null)
            {
                totalSales.Value += sale.Quantity * sale.UnitPrice;
            }
            else
            {
                totalSales = new TotalSales { Value = sale.Quantity * sale.UnitPrice };
                _context.TotalSales.Add(totalSales);
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSale), new { id = sale.SaleId }, sale);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSale(int id, SaleRequest saleRequest)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductName == saleRequest.ProductName);
            if (product == null)
            {
                return BadRequest("Invalid product name.");
            }

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CatName == saleRequest.CatName);
            if (category == null)
            {
                return BadRequest("Invalid category name.");
            }

            sale.ProductId = product.ProductId;
            sale.CategoryId = category.CategoryId;
            sale.Quantity = saleRequest.Quantity;
            sale.UnitPrice = saleRequest.UnitPrice;
            sale.Date = saleRequest.Date;
            sale.ClientName = saleRequest.ClientName;

            _context.Entry(sale).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SaleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("GetTotalSales")]
        public async Task<ActionResult<decimal>> GetTotalSales()
        {
            var totalSales = await _context.TotalSales.FirstOrDefaultAsync();
            if (totalSales == null)
            {
                return NotFound("Total sales record not found.");
            }

            return Ok(totalSales.Value);
        }

        [HttpGet("GetTotalSalesPrice")]
        public IActionResult GetTotalSalesPrice()
        {
            var totalSalePrice = _context.Sales.Sum(s => s.Quantity * s.UnitPrice);
            return Ok(totalSalePrice);
        }

        [HttpGet("latest")]
        public async Task<ActionResult<Sale>> GetLatestSale()
        {
            var sale = await _context.Sales.OrderByDescending(s => s.SaleId).FirstOrDefaultAsync();

            if (sale == null)
            {
                return NotFound();
            }

            return Ok(sale);
        }

        [HttpGet("GetProductPrice")]
        public async Task<ActionResult<decimal>> GetProductPrice(string productName)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductName == productName);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            return product.Price;
        }

        [HttpGet("GetProductStock")]
        public IActionResult GetProductStock(string ProductName)
        {
            var product = _context.Products
                .Where(p => p.ProductName == ProductName)
                .Select(p => new 
                { 
                    AvailableStock = _context.Purchases
                        .Where(purchase => purchase.ProductId == p.ProductId)
                        .Sum(purchase => purchase.Quantity) 
                        - _context.Sales
                        .Where(sale => sale.ProductId == p.ProductId)
                        .Sum(sale => sale.Quantity)
                })
                .FirstOrDefault();

            if (product == null)
            {
                return NotFound("Stock du produit non trouvé");
            }

            return Ok(product.AvailableStock);
        }

        [HttpGet("{id}/quantity")]
        public ActionResult<int> GetSaleQuantity(int id)
        {
            var sale = _context.Sales.Find(id);
            if (sale == null)
            {
                return NotFound();
            }
            return sale.Quantity;
        }

        [HttpGet("quantity")]
        public ActionResult<int> GetSalesQuantityByProductAndCategory(string productName, string categoryName)
        {
            var quantity = _context.Sales
                .Where(s => s.Product.ProductName == productName && s.Category.CatName == categoryName)
                .Sum(s => s.Quantity);

            return quantity;
        }

        private bool SaleExists(int id)
        {
            return _context.Sales.Any(e => e.SaleId == id);
        }
    }
}
