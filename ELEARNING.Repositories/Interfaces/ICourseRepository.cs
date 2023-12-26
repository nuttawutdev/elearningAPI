using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ELEARNING.Repositories.Entities;

namespace ELEARNING.Repositories.Interfaces
{
    public interface ICourseRepository
    {
        Task<sp_insert_course> InsertTHCourse(THCourse newCource);
        Task<sp_insert_course_section> InsertTDCourseSection(TDCourseSection request);
        Task<sp_insert_course_video> InsertTDCourseVideo(TDCourseVideo request);
        Task<List<sp_get_course>> GetCourse();
        Task<sp_get_course_by_id> GetCourseByID(Guid courseID);
        Task<List<sp_get_course_section>> GetCourseSection(Guid courseID);
        Task<List<sp_get_course_video>> GetCourseVideo(Guid courseID);
    }
}