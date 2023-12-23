using System.Threading.Tasks;
using ELEARNING.Repositories.Context;
using ELEARNING.Repositories.Interfaces;
using ELEARNING.Services.Models.Request;
using DapperExtensions;
using ELEARNING.Repositories.Entities;

namespace ELEARNING.Repositories.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly DBContext _context;
        public CourseRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<bool> InsertTHCourse(THCourse request)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.InsertAsync(request);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> InsertTDCourseSection(TDCourseSection request)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.InsertAsync(request);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> InsertTDCourseVideo(TDCourseVideo request)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.InsertAsync(request);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}