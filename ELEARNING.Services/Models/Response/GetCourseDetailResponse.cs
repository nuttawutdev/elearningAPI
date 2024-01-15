using System;
using System.Collections.Generic;

namespace ELEARNING.Services.Models.Response
{
    public class GetCourseDetailResponse : BaseResponse
    {
        public CourseDetailData data { get; set; }
    }

    public class CourseDetailData
    {
        public string courseID { get; set; }
        public string courseName { get; set; }
        public string secondCourseName { get; set; }
        public string courseDescription { get; set; }
        public string createBy { get; set; }
        public DateTime createDate { get; set; }
        public decimal price { get; set; }
        public string remark { get; set; }
        public string linkCourseIntroductionVideo { get; set; }
        public List<CourseSection> courseSection { get; set; }
    }

    public class CourseSection
    {
        public int sectionNumber { get; set; }
        public string sectionName { get; set; }
    }
}