using System;
using System.Threading.Tasks;
using ELEARNING.Services.Models.Request;
using ELEARNING.Services.Models.Response;

namespace ELEARNING.Services.Interfaces
{
    public interface ICourseService
    {
        Task<SaveCourseResponse> SaveCourse(SaveCourseRequest request);
        Task<GetAllCourseResponse> GetAllCourse(GetAllCourseRequest request);
        Task<GetCourseDetailResponse> GetCourseDetail(Guid courseID);
        Task<GetMyCourseDetailResponse> GetMyCourseDetail(GetMyCourseDetailRequest request);
        Task<GetInstructorCourseDetailResponse> GetInstructorCourseDetail(GetMyCourseDetailRequest request);
        Task<GetAllCourseInstructorResponse> GetAllCourseInstructor(GetAllCourseInstructorRequest request);
    }
}