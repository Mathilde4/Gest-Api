using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestStockAPI.Models;

namespace GestStockAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SuppliersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliers()
        {
            return await _context.Suppliers
                         .Where(s => !s.IsDeleted)
                         .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Supplier>> GetSupplier(int Id)
        {
            var supplier = await _context.Suppliers.FindAsync(Id);

            if (supplier == null)
            {
                return NotFound();
            }

            return supplier;
        }

        [HttpPost]
        public async Task<ActionResult<Supplier>> PostSupplier([FromBody] Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSuppliers), new { id = supplier.SupplierId }, supplier);
        }
        
       [HttpPut("{id}")]
public IActionResult UpdateSupplier(int id, [FromBody] Supplier supplier)
{
    var existingSupplier = _context.Suppliers.FirstOrDefault(s => s.SupplierId == id);
    if (existingSupplier == null)
    {
        return NotFound();
    }

    existingSupplier.SupplierName = supplier.SupplierName;
    existingSupplier.SupplierFirstName = supplier.SupplierFirstName;
    existingSupplier.SupplierAddress = supplier.SupplierAddress;
    existingSupplier.Phone = supplier.Phone;
    existingSupplier.Price = supplier.Price;

    _context.SaveChanges();

    return Ok(existingSupplier);
}



        [HttpPut("{id}/delete")]
public IActionResult DeleteSupplier(int id)
{
    var supplier = _context.Suppliers.Find(id);
    if (supplier == null)
    {
        return NotFound();
    }

    supplier.IsDeleted = true;
    _context.SaveChanges();

    return NoContent();
}



        private bool SupplierExists(int id)
        {
            return _context.Suppliers.Any(s => s.SupplierId == id);
        }

        [HttpPut("SoftDeleteSupplier/{id}")]
public async Task<IActionResult> SoftDeleteSupplier(int id)
{
    var supplier = await _context.Suppliers.FindAsync(id);
    if (supplier == null)
    {
        return NotFound();
    }

    supplier.IsDeleted = true;
    _context.Entry(supplier).State = EntityState.Modified;

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!SupplierExists(id))
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


[HttpGet("GetSuppliers")]
public IActionResult GetSuppliers(bool showDeleted = false) {
    var suppliers = _context.Suppliers.Where(s => s.IsDeleted == showDeleted).ToList();
    return Ok(suppliers);
}



    }
}
