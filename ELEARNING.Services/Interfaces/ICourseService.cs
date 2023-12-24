using System.Threading.Tasks;
using ELEARNING.Services.Models.Request;
using ELEARNING.Services.Models.Response;

namespace ELEARNING.Services.Interfaces
{
    public interface ICourseService
    {
        Task<CreateCourseResponse> CreateCourse(CreateCourseRequest request);
        Task<GetAllCourseResponse> GetAllCourse(GetAllCourseRequest request);
    }
}