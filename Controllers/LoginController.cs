using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microbiology.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        private readonly IConfiguration _config;

        public LoginController(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
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


        private string GenerateJwtToken(Users user)
        {
            var key = Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.Id.ToString()) 
            };

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["JwtSettings:ExpiryMinutes"])),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("logging")]
        public async Task<ActionResult<logins>> userLogin([FromBody] logins log)
        {
            var existingUser =  await _context.Login.FirstOrDefaultAsync(u => u.UserName == log.UserName);

            if (existingUser == null)
            {
                return NotFound("User not found"); 
            }

            using (var hmac = new HMACSHA512(existingUser.PasswordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(log.Password));

                if (computedHash.SequenceEqual(existingUser.PasswordHash))
                {
                    var token = GenerateJwtToken(existingUser);
                    return Ok(new { message = "Login successful", user = existingUser, token });
                }
                else
                {
                    return Unauthorized("Invalid password");
                }
            }
        }
    }
}
