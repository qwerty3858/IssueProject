using IssueProject.Common;
using IssueProject.Entity;
using IssueProject.Entity.Context;
using IssueProject.Filters;
using IssueProject.Models.Department;
using IssueProject.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        _2Mes_ConceptualContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(_2Mes_ConceptualContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpGet]
        public async Task<Result<List<UserSummary>>> GetUsers()
        {
            try
            {

                var vResult = await _context.Users
                           .Where(x => x.Deleted == true)
                           .Include(x => x.Department)
                           .Include(x => x.Role)
                           .Select(x => new UserSummary
                           {
                               DepartmentName = x.Department.Definition,
                               RoleName = x.Role.Definition,
                               FullName = x.FullName,
                               EmailAddress = x.EmailAddress
                           }).ToListAsync();

                return Result<List<UserSummary>>.PrepareSuccess(vResult);

            }
            catch (Exception vEx)
            {
                _logger.LogInformation($"Users List Error: {vEx.Message}");
                return Result<List<UserSummary>>.PrepareFailure(vEx.Message);

            }

        }
        [HttpPost]
        public async Task<Result> AddUser(UserInfo userInfo)
        {
            try
            {
                var vUser = new User
                {
                    DepartmentId = userInfo.DepartmentId,
                    RoleId = userInfo.RoleId,
                    FullName = userInfo.FullName,
                    Password = userInfo.Password,
                    EmailAddress = userInfo.EmailAddress,
                    Deleted = false,
                };

                await _context.Users.AddAsync(vUser);
                await _context.SaveChangesAsync();

                return Result.PrepareSuccess();
            }
            catch (Exception vEx)
            {
                _logger.LogInformation($"Users Add Error: {vEx.Message}");
                return Result.PrepareFailure(vEx.Message);
            }
        }

        [HttpPut]
        public async Task<Result<User>> UpdateUser(UserInfo userInfo)
        {
            try
            {
                var vUser =await _context.Users.FirstOrDefaultAsync(x => x.Id == userInfo.Id);
                if(vUser == null)
                {
                    return Result<User>.PrepareFailure($"{userInfo.Id}'li veri Bulunamadı.");
                }
                vUser.DepartmentId = userInfo.DepartmentId;
                vUser.RoleId = userInfo.RoleId;
                vUser.FullName = userInfo.FullName;
                vUser.Password = userInfo.Password;
                vUser.EmailAddress = userInfo.EmailAddress;

                await _context.SaveChangesAsync();

                return Result<User>.PrepareSuccess(vUser);
            }
            catch (Exception vEx)
            {
                _logger.LogInformation($"Users Update Error: {vEx.Message}");
                return Result<User>.PrepareFailure(vEx.Message);
                
            }
                
        }

        [HttpDelete]
        [Security]
        public async Task<Result> DeleteUser(int id)
        {
            try
            {
                var vDelete = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

                vDelete.Deleted = false;

                if (vDelete == null)
                {
                    return Result.PrepareFailure("Kullanıcı bulunamadı");
                }
                return Result.PrepareSuccess("Kullanıcı Silme İşlemi Gerçekleşti");
            }
            catch (Exception vEx)
            {
                _logger.LogInformation($"Users Delete Error: {vEx.Message}");
                return Result.PrepareFailure(vEx.Message);
            }
        }
    }
}
