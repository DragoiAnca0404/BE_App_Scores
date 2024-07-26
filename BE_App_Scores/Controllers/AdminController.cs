using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE_App_Scores.Controllers
{
    [Authorize(Roles ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        [HttpGet("employees")]

        public IEnumerable<string> Get() {
        return new List<string> {"Ahmed", "Ali", "Ahsan" };
        }
    }
}
