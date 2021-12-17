using IssueProject.Common;
using IssueProject.Entity;
using IssueProject.Entity.Context;
using IssueProject.Models.Department;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.Services
{
    public class DepartmentService
    {
        _2Mes_ConceptualContext _context;
        ILogger<DepartmentService> _logger;

        public DepartmentService(_2Mes_ConceptualContext context, ILogger<DepartmentService> logger)
        {
            _context = context;
            _logger = logger;

        }

        public async Task<Result<List<DepartmentInfo>>> GetListDepartments()
        {
            try
            {
                var vResult = await _context.Departments.Where(x=>x.Id>1).Select(x => new DepartmentInfo
                {
                    Id = x.Id,
                    Definition = x.Definition

                }).ToListAsync();

                return Result<List<DepartmentInfo>>.PrepareSuccess(vResult);
            }
            catch (Exception vEx)
            {
                _logger.LogInformation($"Department List Error: {vEx.Message}");
                return Result<List<DepartmentInfo>>.PrepareFailure(vEx.Message);
            }
        }

        public async Task<Result<Department>> AddDepartment(DepartmentInfo departmentInfo)
        {

            try
            {
                var vDepartment = new Department
                {
                    Definition = departmentInfo.Definition
                };
                _context.Departments.Add(vDepartment);
                await _context.SaveChangesAsync();
                return Result<Department>.PrepareSuccess(vDepartment);
            }
            catch (Exception vEx)
            {
                _logger.LogInformation($"Department Add Error: {vEx.Message}");
                return Result<Department>.PrepareFailure(vEx.Message);
            }
        }

        public async Task<Result<Department>> UpdateDepartment(DepartmentInfo departmentInfo)
        {
            try
            {
                var vUpdate = await _context.Departments.FirstOrDefaultAsync(x => x.Id == departmentInfo.Id);

                if (vUpdate == null)
                {
                    return Result<Department>.PrepareFailure($"{departmentInfo.Id}'li veri bulunamadı.");
                }

                vUpdate.Definition = departmentInfo.Definition;

                await _context.SaveChangesAsync();

                return Result<Department>.PrepareSuccess(vUpdate);
            }
            catch (Exception vEx)
            {
                _logger.LogInformation($"Department Update Error: {vEx.Message}");
                return Result<Department>.PrepareFailure(vEx.Message);
            }
        }

        public async Task<Result<Department>> DeleteDepartment(int id)
        {
            try
            {
                var vDelete = _context.Departments.FirstOrDefault(x => x.Id == id);
                if (vDelete == null)
                {
                    return Result<Department>.PrepareFailure($"{vDelete.Id}'li kullanıcı bulunamadı.");
                }
                _context.Departments.Remove(vDelete);
                await _context.SaveChangesAsync();
                return Result<Department>.PrepareSuccess(vDelete);
            }
            catch (Exception vEx)
            {
                _logger.LogInformation($"Department Delete Error: {vEx.Message}");
                return Result<Department>.PrepareFailure(vEx.Message);
            }
        }
    }
}
