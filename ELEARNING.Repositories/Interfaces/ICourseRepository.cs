using System.Threading.Tasks;
using ELEARNING.Repositories.Entities;
using ELEARNING.Services.Models.Request;

namespace ELEARNING.Repositories.Interfaces
{
    public interface ICourseRepository
    {
        Task<bool> InsertTHCourse(THCourse newCource);
    }
}