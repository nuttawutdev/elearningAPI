using System.Threading.Tasks;
using ELEARNING.Services.Interfaces;
using ELEARNING.Services.Models.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static ELEARNING.Services.Models.Commom.AppEnum;

namespace ELEARNING.API.Controllers
{

    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpPost]
        [Route("course/v1/create-course")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateCourseResponse))]
        public async Task<IActionResult> CreateCourse([FromForm] CreateCourseRequest request)
        {
            var response = new CreateCourseResponse();
            try
            {
                response = await _courseService.CreateCourse(request);
            }
            catch (System.Exception ex)
            {
                response.data = null;
                response.responseCode = ResponseCode.InternalError.Text();
                response.responseMessage = "เกิดข้อผิดพลาดในระบบ";
            };
            return Ok(response);
        }


    }
}