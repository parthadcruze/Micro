using System.Security.Cryptography;
using System.Text;
using Microbiology.Data;
using Microbiology.DTO;
using Microbiology.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Microbiology.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly DataContext _context;

        public LoginController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("signin")]
        public async Task<ActionResult<logins>> Login_creation([FromBody] logins log)
        {
            Users users = new Users();
            using (var hmac = new HMACSHA512())
            {
                users.UserName = log.UserName;
                users.PasswordSalt = hmac.Key;
                users.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(log.Password));
            }

            await _context.AddAsync(users);
            await _context.SaveChangesAsync();

            return Ok(users);
        }

        [HttpPost("logging")]
        public async Task<ActionResult<logins>> userLogin([FromBody] logins log)
        {
            var existingUser =  await _context.Login.FirstOrDefaultAsync(u => u.UserName.ToLower() == log.UserName);

            if (existingUser == null)
            {
                return NotFound("User not found"); 
            }

            using (var hmac = new HMACSHA512(existingUser.PasswordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(log.Password));

                if (computedHash.SequenceEqual(existingUser.PasswordHash))
                {
                    return Ok(new { message = "Login successful", user = existingUser });
                }
                else
                {
                    return Unauthorized("Invalid password");
                }
            }

        }
    }
}
