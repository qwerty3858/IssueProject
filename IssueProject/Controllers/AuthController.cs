using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using IssueProject.Models.User;
using IssueProject.Services;
using Microsoft.Extensions.Logging;

namespace IssueProject.Controllers
{

    [Route("api/Auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IConfiguration _configuration;
        private AuthService _authService;
        private ILogger<AuthController> _logger;

        public AuthController( IConfiguration configuration,AuthService authService, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _authService = authService;
            _logger = logger;
        } 

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginQuery loginQuery)
        {
          
                var vLogin = await _authService.AuthForLogIn(loginQuery);
                if(vLogin == null)
                    _logger.LogError("Login İşlemi Başarısız. ");
                return Ok(vLogin);

           
        }

    }
  
}
