using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.Hangfire.Interface
{
    public interface IJob
    {
        public Task JobMethod();
    }
}
