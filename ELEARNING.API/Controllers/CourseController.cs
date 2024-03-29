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
        [Route("course/v1/save-course")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SaveCourseResponse))]
        public async Task<IActionResult> SaveCourse([FromForm] SaveCourseRequest request)
        {
            var response = new SaveCourseResponse();
            try
            {
                response = await _courseService.SaveCourse(request);
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

        [HttpPost]
        [Route("course/v1/get-my-course-detail")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetMyCourseDetailResponse))]
        public async Task<IActionResult> GetMyCourseDetail([FromBody] GetMyCourseDetailRequest request)
        {
            var response = new GetMyCourseDetailResponse();
            try
            {
                response = await _courseService.GetMyCourseDetail(request);
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
        [Route("course/v1/get-instructor-course-detail")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetInstructorCourseDetailResponse))]
        public async Task<IActionResult> GetInstructorCourseDetail([FromBody] GetMyCourseDetailRequest request)
        {
            var response = new GetInstructorCourseDetailResponse();
            try
            {
                response = await _courseService.GetInstructorCourseDetail(request);
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
        [Route("course/v1/get-all-course-instructor")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllCourseInstructorResponse))]
        public async Task<IActionResult> GetAllCourseInstructor([FromBody] GetAllCourseInstructorRequest request)
        {
            var response = new GetAllCourseInstructorResponse();
            try
            {
                response = await _courseService.GetAllCourseInstructor(request);
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
        [Route("course/v1/get-all-my-course")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllCourseResponse))]
        public async Task<IActionResult> GetAllMyCourse([FromBody] GetAllMyCourseRequest request)
        {
            var response = new GetAllCourseResponse();
            try
            {
                response = await _courseService.GetAllMyCourse(request);
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
        [DisableRequestSizeLimit]
        [Route("course/v1/delete-course")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DeleteCourseResponse))]
        public async Task<IActionResult> DeleteCourse([FromBody] DeleteCourseRequest request)
        {
            var response = new DeleteCourseResponse();
            try
            {
                response = await _courseService.DeleteCourse(request);
            }
            catch (System.Exception ex)
            {
                response.responseCode = ResponseCode.InternalError.Text();
                response.responseMessage = "เกิดข้อผิดพลาดในระบบ";
            };
            return Ok(response);
        }

    }
}