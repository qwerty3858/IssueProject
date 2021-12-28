using IssueProject.Common;
using IssueProject.Entity.Context;
using IssueProject.Models.Role;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.Services
{
    public class RoleService
    {
        _2Mes_ConceptualContext _context;
        private ILogger<RoleService> _logger;

        public RoleService(_2Mes_ConceptualContext context, ILogger<RoleService> logger)
        {
            _context = context;
            _logger = logger;
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
                _logger.LogError(vEx, "Role List Error");
                return Result<List<RoleInfo>>.PrepareFailure(vEx.Message);
            }
        }
    }
}
