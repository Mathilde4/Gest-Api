using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using GestStockAPI.Models;

namespace GestStockAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Inventory
        [HttpGet]
        public ActionResult<IEnumerable<InventoryResult>> GetInventoryQuantities()
        {
            var results = _context.Products
                .Include(p => p.Category)
                .Select(p => new InventoryResult
                {
                    ProductName = p.ProductName,  // Utilisation de ProductName
                    CategoryName = p.Category.CatName,  // Utilisation de CatName
                    PurchaseQuantity = _context.Purchases
                        .Where(pur => pur.ProductId == p.ProductId)
                        .Sum(pur => pur.Quantity),
                    SalesQuantity = _context.Sales
                        .Where(s => s.ProductId == p.ProductId)
                        .Sum(s => s.Quantity),
                    RemainingQuantity = _context.Purchases
                        .Where(pur => pur.ProductId == p.ProductId)
                        .Sum(pur => pur.Quantity) - _context.Sales
                        .Where(s => s.ProductId == p.ProductId)
                        .Sum(s => s.Quantity)
                })
                .ToList();

            return Ok(results);
        }
    }
}
