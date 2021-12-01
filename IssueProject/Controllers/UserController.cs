using IssueProject.Models.User;
using IssueProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IssueProject.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class UserController : ControllerBase
    {
            UserService _userService;

            public UserController(UserService userService)
            {
                _userService = userService;
            }

            [HttpGet("Get")]
            public async Task<IActionResult> GetUserList()
            {
                //int vUserId = Convert.ToInt32(User.FindFirst("sub").Value);

                var vResult = await _userService.GetUsers();
                return Ok(vResult);
            }
        [HttpGet("GetByUserId/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var vResult = await _userService.GetUserByUserId(id);
            return Ok(vResult);
        }


            [HttpPost("Add")]
            public async Task<IActionResult> AddUser(UserInfo userInfo)
            {
                var vResult = await _userService.AddUser(userInfo);
                return Ok(vResult);
            }
            
            [HttpPut("Update")]
            public async Task<IActionResult> UpdateUser(UserInfo userInfo)
            {

                var vResult = await _userService.UpdateUser(userInfo);
                return Ok(vResult);
            }

            [HttpDelete("Delete/{id}")]
            public async Task<IActionResult> DeleteUser(int id)
            {
                var delete = await _userService.DeleteUser(id);
                return Ok(delete);
            }
        }
    }
