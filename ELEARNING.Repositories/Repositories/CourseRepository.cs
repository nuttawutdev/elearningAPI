using System.Threading.Tasks;
using ELEARNING.Repositories.Context;
using ELEARNING.Repositories.Interfaces;
using DapperExtensions;
using ELEARNING.Repositories.Entities;
using System;
using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace ELEARNING.Repositories.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly DBContext _context;
        public CourseRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<sp_insert_course> InsertTHCourse(THCourse request)
        {
            try
            {
                string store = "sp_insert_course";
                var queryParam = new DynamicParameters();
                queryParam.Add("@ID", request.ID);
                queryParam.Add("@Course_Name", request.Course_Name);
                queryParam.Add("@Second_Course_Name", request.Second_Course_Name);
                queryParam.Add("@Course_Desc", request.Course_Desc);
                queryParam.Add("@Link_Cover_Course_Video_ID", request.Link_Cover_Course_Video_ID);
                queryParam.Add("@Video_ID", request.Video_ID);
                queryParam.Add("@Level_ID", request.Level_ID);
                queryParam.Add("@Price", request.Price);
                queryParam.Add("@Remark", request.Remark);
                queryParam.Add("@Create_By", request.Create_By);
                queryParam.Add("@Create_Date", request.Create_Date);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryFirstAsync<sp_insert_course>(store, queryParam, commandType: System.Data.CommandType.StoredProcedure);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<sp_insert_course_section> InsertTDCourseSection(TDCourseSection request)
        {
            try
            {
                string store = "sp_insert_course_section";
                var queryParam = new DynamicParameters();
                queryParam.Add("@ID", request.ID);
                queryParam.Add("@Course_ID", request.Course_ID);
                queryParam.Add("@Section_Name", request.Section_Name);
                queryParam.Add("@Section_Number", request.Section_Number);
                queryParam.Add("@Create_By", request.Create_By);
                queryParam.Add("@Create_Date", request.Create_Date);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryFirstAsync<sp_insert_course_section>(store, queryParam, commandType: System.Data.CommandType.StoredProcedure);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<sp_insert_course_video> InsertTDCourseVideo(TDCourseVideo request)
        {
            try
            {
                string store = "sp_insert_course_video";
                var queryParam = new DynamicParameters();
                queryParam.Add("@ID", request.ID);
                queryParam.Add("@Course_ID", request.Course_ID);
                queryParam.Add("@Course_Section_ID", request.Course_Section_ID);
                queryParam.Add("@Video_Name", request.Video_Name);
                queryParam.Add("@Video_ID", request.Video_ID);
                queryParam.Add("@Video_Number", request.Video_Number);
                queryParam.Add("@Create_By", request.Create_By);
                queryParam.Add("@Create_Date", request.Create_Date);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryFirstAsync<sp_insert_course_video>(store, queryParam, commandType: System.Data.CommandType.StoredProcedure);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<sp_get_course>> GetCourse()
        {
            try
            {
                string store = "sp_get_course";

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<sp_get_course>(store, commandType: System.Data.CommandType.StoredProcedure);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<sp_get_course_by_id> GetCourseByID(Guid courseID)
        {
            try
            {
                string store = "sp_get_course_by_id";
                var queryParam = new DynamicParameters();
                queryParam.Add("@CourseID", courseID);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryFirstAsync<sp_get_course_by_id>(store, queryParam, commandType: System.Data.CommandType.StoredProcedure);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<sp_get_course_section>> GetCourseSection(Guid courseID)
        {
            try
            {
                string store = "sp_get_course_section";
                var queryParam = new DynamicParameters();
                queryParam.Add("@CourseID", courseID);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<sp_get_course_section>(store, queryParam, commandType: System.Data.CommandType.StoredProcedure);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<sp_get_course_video>> GetCourseVideo(Guid courseID)
        {
            try
            {
                string store = "sp_get_course_video";
                var queryParam = new DynamicParameters();
                queryParam.Add("@CourseID", courseID);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<sp_get_course_video>(store, queryParam, commandType: System.Data.CommandType.StoredProcedure);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<sp_update_course> UpdateTHCourse(THCourse request)
        {
            try
            {
                string store = "sp_update_course";
                var queryParam = new DynamicParameters();
                queryParam.Add("@ID", request.ID);
                queryParam.Add("@Course_Name", request.Course_Name);
                queryParam.Add("@Second_Course_Name", request.Second_Course_Name);
                queryParam.Add("@Course_Desc", request.Course_Desc);
                queryParam.Add("@Link_Cover_Course_Video_ID", request.Link_Cover_Course_Video_ID);
                queryParam.Add("@Video_ID", request.Video_ID);
                queryParam.Add("@Level_ID", request.Level_ID);
                queryParam.Add("@Price", request.Price);
                queryParam.Add("@Remark", request.Remark);
                queryParam.Add("@Update_By", request.Update_By);
                queryParam.Add("@Update_Date", request.Update_Date);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryFirstAsync<sp_update_course>(store, queryParam, commandType: System.Data.CommandType.StoredProcedure);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<sp_update_course_section> UpdateTDCourseSection(TDCourseSection request)
        {
            try
            {
                string store = "sp_update_course_section";
                var queryParam = new DynamicParameters();
                queryParam.Add("@ID", request.ID);
                queryParam.Add("@Section_Name", request.Section_Name);
                queryParam.Add("@Section_Number", request.Section_Number);
                queryParam.Add("@Update_By", request.Update_By);
                queryParam.Add("@Update_Date", request.Update_Date);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryFirstAsync<sp_update_course_section>(store, queryParam, commandType: System.Data.CommandType.StoredProcedure);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<sp_update_course_video> UpdateTDCourseVideo(TDCourseVideo request)
        {
            try
            {
                string store = "sp_update_course_video";
                var queryParam = new DynamicParameters();
                queryParam.Add("@ID", request.ID);
                queryParam.Add("@Video_Name", request.Video_Name);
                queryParam.Add("@Video_ID", request.Video_ID);
                queryParam.Add("@Update_By", request.Update_By);
                queryParam.Add("@Update_Date", request.Update_Date);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryFirstAsync<sp_update_course_video>(store, queryParam, commandType: System.Data.CommandType.StoredProcedure);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<sp_delete_course> DeleteCourse(Guid courseID)
        {
            try
            {
                string store = "sp_delete_course";
                var queryParam = new DynamicParameters();
                queryParam.Add("@ID", courseID);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryFirstAsync<sp_delete_course>(store, queryParam, commandType: System.Data.CommandType.StoredProcedure);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<sp_delete_course_section> DeleteCourseSection(Guid courseID)
        {
            try
            {
                string store = "sp_delete_course_section";
                var queryParam = new DynamicParameters();
                queryParam.Add("@ID", courseID);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryFirstAsync<sp_delete_course_section>(store, queryParam, commandType: System.Data.CommandType.StoredProcedure);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<sp_delete_course_video> DeleteCourseVideo(Guid courseID)
        {
            try
            {
                string store = "sp_delete_course_video";
                var queryParam = new DynamicParameters();
                queryParam.Add("@ID", courseID);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryFirstAsync<sp_delete_course_video>(store, queryParam, commandType: System.Data.CommandType.StoredProcedure);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<sp_get_all_course_instructor>> GetAllCourseInstructor(string userID)
        {
            try
            {
                string store = "sp_get_all_course_instructor";
                var queryParam = new DynamicParameters();
                queryParam.Add("@UserID", userID);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<sp_get_all_course_instructor>(store, queryParam, commandType: System.Data.CommandType.StoredProcedure);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}