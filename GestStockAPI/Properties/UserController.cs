using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestStockAPI.Models;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }


    [HttpPost("register")]
    public async Task<IActionResult> Register(User user)
    {
        // Check if username already exists
        if (_context.Users.Any(u => u.UserName == user.UserName))
        {
            return BadRequest("Username already exists");
        }

        // Hash password before saving (use a library like BCrypt)
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
    }




    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User loginUser)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == loginUser.UserName);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginUser.PasswordHash, user.PasswordHash))
        {
            return Unauthorized("Invalid credentials");
        }

        // Generate JWT token (not covered in this example)
        return Ok(new { Token = "jwt-token-placeholder" });
    }

    [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int UserId)
        {
            var user = await _context.Users.FindAsync(UserId);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

    [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (id != user.UserId)
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
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        private bool UserExists(int id)
        {
            return _context.Users.Any(u => u.UserId == id);
        }
    
}
