using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using IssueProject.Helpers.TokenOperations;
using IssueProject.Entity;
using IssueProject.Models.User;
using IssueProject.Common;
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

        //[HttpPost("register")]
        //public async Task<IActionResult> Register([FromBody] UserInfo userInfo)
        //{

        //    if (await _authRepository.UserExists(userInfo.FullName))
        //    {
        //        ModelState.AddModelError("UserName", "Username already exists");
        //    }
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var userToCreate = new User
        //    {
        //        DepartmentId = userInfo.DepartmentId,
        //        RoleId = userInfo.RoleId,
        //        FullName = userInfo.FullName,
        //        Password = userInfo.Password,
        //        EmailAddress = userInfo.EmailAddress,
        //        Deleted = false,

        //    };

        //    var createdUser = await _authRepository.Register(userToCreate, userInfo.Password);
        //    return StatusCode(201);

        //}

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginQuery loginQuery)
        {
          
                var vLogin = await _authService.AuthForLogIn(loginQuery);
                if(vLogin == null)
                    _logger.LogInformation("Login İşlemi Başarısız. ");
                return Ok(vLogin);

           
        }

    }
  
}
