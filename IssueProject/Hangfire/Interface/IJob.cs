using System.Threading.Tasks;

namespace IssueProject.Hangfire.Interface
{
    public interface IJob
    {
        public Task JobMethod();
    }
}
