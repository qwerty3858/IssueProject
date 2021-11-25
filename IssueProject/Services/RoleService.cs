using IssueProject.Common;
using IssueProject.Entity.Context;
using IssueProject.Models.Role;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.Services
{
    public class RoleService
    {
        _2Mes_ConceptualContext _context;

        public RoleService(_2Mes_ConceptualContext context)
        {
            _context = context;
        }

        
        public async Task<Result<List<RoleInfo>>> GetRole()
        {
            try
            {
                var vResult = await _context.Roles.Select(x => new RoleInfo
                {
                    Id = x.Id,
                    Definition = x.Definition
                }).ToListAsync();

                return Result<List<RoleInfo>>.PrepareSuccess(vResult);

            }
            catch (Exception vEx)
            {

                return Result<List<RoleInfo>>.PrepareFailure(vEx.Message);
            }
        }
    }
}
