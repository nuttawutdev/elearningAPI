using System.Threading.Tasks;
using ELEARNING.Repositories.Entities;

namespace ELEARNING.Repositories.Interfaces
{
    public interface ICourseRepository
    {
        Task<sp_insert_course> InsertTHCourse(THCourse newCource);
        Task<sp_insert_course_section> InsertTDCourseSection(TDCourseSection request);
        Task<sp_insert_course_video> InsertTDCourseVideo(TDCourseVideo request);
    }
}