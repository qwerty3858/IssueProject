using IssueProject.Common;
using IssueProject.Controllers;
using IssueProject.Entity;
using IssueProject.Entity.Context;
using IssueProject.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.Services
{
    public class UserService
    {
        _2Mes_ConceptualContext _context;
        ILogger<UserController> _logger;

        public UserService(_2Mes_ConceptualContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<Result<List<UserSummary>>> GetUsers()
        {
            try
            {
                var vResult = await _context.Users
                           .Where(x => x.Deleted == false)
                           .Include(x => x.Department)
                           .Include(x => x.Role)
                           .Select(x => new UserSummary
                           {
                               Id = x.Id,
                               DepartmentName = x.Department.Definition,
                               RoleName = x.Role.Definition,
                               FullName = x.FullName,
                               EmailAddress = x.EmailAddress,
                               IsManager=x.IsManager,
                               IsKeyUser = x.IsKeyUser
                           }).ToListAsync();

                return Result<List<UserSummary>>.PrepareSuccess(vResult);

            }
            catch (Exception vEx)
            {
                _logger.LogError(vEx, "Users List Error");
                return Result<List<UserSummary>>.PrepareFailure(vEx.Message);

            }

        }

        public async Task<Result<UserInfo>> GetUserByUserId(int id)
        {

            try
            {
                var vResult = await _context.Users.Include(a=>a.Role).Include(d=>d.Department)
                .Select(x => new UserInfo
                {
                    Id = x.Id,
                    DepartmentId = x.DepartmentId,
                    RoleId = x.RoleId,
                    FullName = x.FullName,
                    Password=x.Password,
                    EmailAddress = x.EmailAddress,
                    DepartmentName=x.Department.Definition,
                    RoleName=x.Role.Definition,
                    IsManager = x.IsManager,
                    IsKeyUser = x.IsKeyUser
                    
                })
                .FirstOrDefaultAsync(x => x.Id == id );

                return Result<UserInfo>.PrepareSuccess(vResult);
            }
            catch (Exception vEx)
            {
                _logger.LogError(vEx, "User Info Error");
                return Result<UserInfo>.PrepareFailure(vEx.Message);
            }
        }

        public async Task<Result<User>> AddUser(UserInfo userInfo)
        {
            try
            {
                var vUserCount =await _context.Users.FirstOrDefaultAsync(x => x.Id == userInfo.Id);
                if(vUserCount != null)
                {
                    return Result<User>.PrepareFailure($"{userInfo.Id}'ye Sahip Kullanıcı Zaten Mevcut!");
                }
                var vUser = new User
                {
                    Id=userInfo.Id,
                    DepartmentId = userInfo.DepartmentId,
                    RoleId = userInfo.RoleId,
                    FullName = userInfo.FullName,
                    Password = userInfo.Password,
                    EmailAddress = userInfo.EmailAddress,
                    Deleted = false,
                    IsManager=userInfo.IsManager,
                    IsKeyUser = userInfo.IsKeyUser

                };

                await _context.Users.AddAsync(vUser);
                await _context.SaveChangesAsync();

                return Result<User>.PrepareSuccess(vUser);
            }
            catch (Exception vEx)
            {
                _logger.LogError(vEx, "User Add Error");
                return Result<User>.PrepareFailure(vEx.Message);
            }
        }

        public async Task<Result<User>> UpdateUser(UserInfo userInfo)
        {
            try
            {
                var vUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == userInfo.Id && x.Deleted==false);
                if (vUser == null)
                {
                    return Result<User>.PrepareFailure($"{userInfo.Id}'li veri Bulunamadı.");
                }
                vUser.DepartmentId = userInfo.DepartmentId;
                vUser.RoleId = userInfo.RoleId;
                vUser.FullName = userInfo.FullName;
                vUser.Password = userInfo.Password;
                vUser.EmailAddress = userInfo.EmailAddress;
                vUser.Deleted = false;
                vUser.IsManager = userInfo.IsManager;

                await _context.SaveChangesAsync();

                return Result<User>.PrepareSuccess(vUser);
            }
            catch (Exception vEx)
            {
                _logger.LogError(vEx, "User Update Error");
                return Result<User>.PrepareFailure(vEx.Message);

            }

        }

        public async Task<Result<User>> DeleteUser(int id)
        {
            try
            {
                var vDelete = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

                if (vDelete == null)
                {
                    return Result<User>.PrepareFailure("Kullanıcı bulunamadı");
                }
                vDelete.Deleted = true;
                await _context.SaveChangesAsync();
                return Result<User>.PrepareSuccess(vDelete);
            }
            catch (Exception vEx)
            {
                _logger.LogError(vEx, "User Delete Error");
                return Result<User>.PrepareFailure(vEx.Message);
            }

        }
    }
}
