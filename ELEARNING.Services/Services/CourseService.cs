using System.Threading.Tasks;
using ELEARNING.Services.Interfaces;
using ELEARNING.Services.Models.Request;

namespace ELEARNING.Services.Services
{
    public class CourseService : ICourseService
    {
        public Task<CreateCourseResponse> CreateCourse(CreateCourseRequest request)
        {
            throw new System.NotImplementedException();
        }

    }
}