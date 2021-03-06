using IssueProject.Models.Department;
using IssueProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IssueProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class DepartmentController : ControllerBase
    {
        DepartmentService _departmentService;

        public DepartmentController(DepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet("Get")]
        public async Task<IActionResult> GetDepartmentsList()
        {
            //int vUserId = Convert.ToInt32(User.FindFirst("sub").Value);

            var vResult = await _departmentService.GetListDepartments();
            return Ok(vResult);
        }


        [HttpPost("Add")]
        public async Task<IActionResult> AddDepartment(DepartmentInfo departmentInfo)
        {
            var vResult = await _departmentService.AddDepartment(departmentInfo);
            return Ok(vResult);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateDepartment(DepartmentInfo departmentInfo)
        {

            var vResult = await _departmentService.UpdateDepartment(departmentInfo);
            return Ok(vResult);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var delete = await _departmentService.DeleteDepartment(id);
            return Ok(delete);
        }
    }
}
