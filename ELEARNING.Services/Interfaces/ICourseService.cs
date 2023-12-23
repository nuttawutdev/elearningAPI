using System.Threading.Tasks;
using ELEARNING.Services.Models.Request;

namespace ELEARNING.Services.Interfaces
{
    public interface ICourseService
    {
        Task<CreateCourseResponse> CreateCourse(CreateCourseRequest request);
    }
}