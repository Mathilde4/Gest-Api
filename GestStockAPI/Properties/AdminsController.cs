using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestStockAPI.Models;

[Route("api/[controller]")]
[ApiController]
public class AdminsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdminsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Admin>>> GetAdmins()
    {
        return await _context.Admins.ToListAsync();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(Admin admin)
    {
        if (_context.Admins.Any(a => a.AdminName == admin.AdminName))
        {
            return BadRequest("Admin name already exists");
        }

        // Hash password before saving (use a library like BCrypt)
        admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(admin.PasswordHash);
        _context.Admins.Add(admin);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAdmin), new { id = admin.AdminId }, admin);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Admin loginAdmin)
    {
        var admin = await _context.Admins
            .FirstOrDefaultAsync(a => a.AdminName == loginAdmin.AdminName);

        if (admin == null || !BCrypt.Net.BCrypt.Verify(loginAdmin.PasswordHash, admin.PasswordHash))
        {
            return Unauthorized("Invalid credentials");
        }

        // Generate JWT token (not covered in this example)
        return Ok(new { Token = "jwt-token-placeholder" });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Admin>> GetAdmin(int id)
    {
        var admin = await _context.Admins.FindAsync(id);

        if (admin == null)
        {
            return NotFound();
        }

        return admin;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAdmin(int id, Admin admin)
    {
        if (id != admin.AdminId)
        {
            return BadRequest();
        }

        _context.Entry(admin).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AdminExists(id))
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
    public async Task<IActionResult> DeleteAdmin(int id)
    {
        var admin = await _context.Admins.FindAsync(id);
        if (admin == null)
        {
            return NotFound();
        }

        _context.Admins.Remove(admin);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private bool AdminExists(int id)
    {
        return _context.Admins.Any(a => a.AdminId == id);
    }
}
