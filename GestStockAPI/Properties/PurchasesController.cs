using Microsoft.AspNetCore.Mvc;
using GestStockAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GestStockAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PurchasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseRequest>>> GetPurchases()
        {
            var purchases = await _context.Purchases
                .Include(p => p.Product)
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Select(p => new PurchaseRequest
                {
                    Quantity = p.Quantity,
                    UnitPrice = p.UnitPrice,
                    Date = p.Date,
                    ProductName = p.Product.ProductName,
                    CatName = p.Category.CatName,
                    SupplierName = p.Supplier.SupplierName,
                })
                .ToListAsync();

            return Ok(purchases);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseRequest>> GetPurchase(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.Product)
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.PurchaseId == id)
                .Select(p => new PurchaseRequest
                {
                    Quantity = p.Quantity,
                    UnitPrice = p.UnitPrice,
                    Date = p.Date,
                    ProductName = p.Product.ProductName,
                    CatName = p.Category.CatName,
                    SupplierName = p.Supplier.SupplierName,
                })
                .FirstOrDefaultAsync();

            if (purchase == null)
            {
                return NotFound();
            }

            return Ok(purchase);
        }

        [HttpPost]
        public async Task<ActionResult<Purchase>> PostPurchase(PurchaseRequest purchaseRequest)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductName == purchaseRequest.ProductName);
            if (product == null)
            {
                return NotFound($"Produit avec le nom '{purchaseRequest.ProductName}' non trouvé.");
            }

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CatName == purchaseRequest.CatName);
            if (category == null)
            {
                return NotFound($"Catégorie avec le nom '{purchaseRequest.CatName}' non trouvée.");
            }

            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.SupplierName == purchaseRequest.SupplierName);
            if (supplier == null)
            {
                return NotFound($"Fournisseur avec le nom '{purchaseRequest.SupplierName}' non trouvé.");
            }

            var purchase = new Purchase
            {
                ProductId = product.ProductId,
                CategoryId = category.CategoryId,
                SupplierId = supplier.SupplierId,
                Quantity = purchaseRequest.Quantity,
                UnitPrice = purchaseRequest.UnitPrice,
                Date = purchaseRequest.Date
            };

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPurchase), new { id = purchase.PurchaseId }, purchase);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPurchase(int id, PurchaseRequest purchaseRequest)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductName == purchaseRequest.ProductName);
            if (product == null)
            {
                return NotFound($"Produit avec le nom '{purchaseRequest.ProductName}' non trouvé.");
            }

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CatName == purchaseRequest.CatName);
            if (category == null)
            {
                return NotFound($"Catégorie avec le nom '{purchaseRequest.CatName}' non trouvée.");
            }

            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.SupplierName == purchaseRequest.SupplierName);
            if (supplier == null)
            {
                return NotFound($"Fournisseur avec le nom '{purchaseRequest.SupplierName}' non trouvé.");
            }

            purchase.Quantity = purchaseRequest.Quantity;
            purchase.UnitPrice = purchaseRequest.UnitPrice;
            purchase.Date = purchaseRequest.Date;
            purchase.ProductId = product.ProductId;
            purchase.CategoryId = category.CategoryId;
            purchase.SupplierId = supplier.SupplierId;

            _context.Entry(purchase).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseExists(id))
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
        public async Task<IActionResult> DeletePurchase(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                return NotFound();
            }

            _context.Purchases.Remove(purchase);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("GetPriceBySupplier/{supplierName}")]
        public async Task<ActionResult<decimal>> GetPriceBySupplier(string supplierName)
        {
            var supplier = await _context.Suppliers
                .Where(s => s.SupplierName == supplierName)
                .FirstOrDefaultAsync();

            if (supplier == null)
            {
                return NotFound($"Fournisseur avec le nom '{supplierName}' non trouvé.");
            }

            return Ok(supplier.Price);  
        }

        [HttpGet("{id}/quantity")]
        public ActionResult<int> GetPurchaseQuantity(int id)
        {
            var purchase = _context.Purchases.Find(id);
            if (purchase == null)
            {
                return NotFound();
            }
            return purchase.Quantity;
        }

        [HttpGet("quantity")]
        public ActionResult<int> GetPurchaseQuantityByProductAndCategory(string productName, string categoryName)
        {
            var quantity = _context.Purchases
                .Where(p => p.Product.ProductName == productName && p.Category.CatName == categoryName)
                .Sum(p => p.Quantity);

            return quantity;
        }


        [HttpGet("GetTotalPurchasePrice")]
        public IActionResult GetTotalPurchasePrice()
        {
            var totalPurchasePrice = _context.Purchases.Sum(p => p.Quantity * p.UnitPrice);
            return Ok(totalPurchasePrice);
        }





        private bool PurchaseExists(int id)
        {
            return _context.Purchases.Any(e => e.PurchaseId == id);
        }
    }
}
