using System;
using System.Collections.Generic;

namespace ELEARNING.Services.Models.Response
{
    public class GetAllCourseResponse : BaseResponse
    {
        public List<CourseData> data { get; set; }
    }

    public class CourseData
    {
        public string courseID { get; set; }
        public string courseName { get; set; }
        public string createBy { get; set; }
        public decimal price { get; set; }
        public string linkCoverCourseVideo { get; set; }
    }
}