using System.Collections.Generic;

namespace ELEARNING.Services.Models.Response
{
    public class GetAllCourseInstructorResponse : BaseResponse
    {
        public List<CourseInstructorData> data { get; set; }
    }

    public class CourseInstructorData
    {
        public string courseID { get; set; }
        public string courseName { get; set; }
        public string createBy { get; set; }
        public decimal price { get; set; }
        public string status { get; set; }
        public string createDate { get; set; }
    }
}