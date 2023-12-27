using System;
using System.Collections.Generic;

namespace ELEARNING.Services.Models.Response
{
    public class GetInstructorCourseDetailResponse : BaseResponse
    {
        public InstructorCourseDetailData data { get; set; }
    }

    public class InstructorCourseDetailData
    {
        public Guid courseID { get; set; }
        public string courseName { get; set; }
        public string secondCourseName { get; set; }
        public string courseDescription { get; set; }
        public string createBy { get; set; }
        public string linkCourseIntroductionVideo { get; set; }
        public string linkCoverCourseVideo { get; set; }
        public decimal price { get; set; }
        public List<InstructorCourseSection> courseSection { get; set; }
    }

    public class InstructorCourseSection
    {
        public Guid courseSectionID { get; set; }
        public int sectionNumber { get; set; }
        public string sectionName { get; set; }
        public List<InstructorCourseVideo> courseVideo { get; set; }
    }

    public class InstructorCourseVideo
    {
        public Guid courseVideoID { get; set; }
        public string videoName { get; set; }
        public string linkCourseVideo { get; set; }
    }
}