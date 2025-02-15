using Microbiology.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microbiology.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class GetDataController : ControllerBase
    {
        private readonly DataContext _context;
        public GetDataController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetData()
        {
            var users = _context.Login.ToList();
            return Ok(users);
        }
    }
}
