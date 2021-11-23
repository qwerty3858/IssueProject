using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.Filters
{
    public class Security : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string UserID = filterContext.HttpContext.Request.Headers["UserID"].ToString();
            
            string UserRole = filterContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Roles")?.Value;
            if (!(UserRole == "SuperAdmin" || UserRole == "Admin"))
            {
                var User = filterContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AUID")?.Value;
                if (User !=UserID)
                {
                    filterContext.Result = new UnauthorizedResult(); //401 döner
                    // Yetkisi olmayan kullanıcılar buraya gelir
                }
            }
        }
    }
}
