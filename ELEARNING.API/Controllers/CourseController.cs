using System;
using System.Threading.Tasks;
using ELEARNING.Services.Interfaces;
using ELEARNING.Services.Models.Request;
using ELEARNING.Services.Models.Response;
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
        [DisableRequestSizeLimit]
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

        [HttpPost]
        [Route("course/v1/get-all-course")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllCourseResponse))]
        public async Task<IActionResult> GetAllCourse([FromBody] GetAllCourseRequest request)
        {
            var response = new GetAllCourseResponse();
            try
            {
                response = await _courseService.GetAllCourse(request);
            }
            catch (System.Exception ex)
            {
                response.data = null;
                response.responseCode = ResponseCode.InternalError.Text();
                response.responseMessage = "เกิดข้อผิดพลาดในระบบ";
            };
            return Ok(response);
        }


        [HttpGet]
        [Route("course/v1/get-course-detail")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCourseDetailResponse))]
        public async Task<IActionResult> GetCourseDetail(Guid courseID)
        {
            var response = new GetCourseDetailResponse();
            try
            {
                response = await _courseService.GetCourseDetail(courseID);
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