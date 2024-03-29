using System;
using System.Collections.Generic;

namespace ELEARNING.Services.Models.Response
{
    public class GetMyCourseDetailResponse : BaseResponse
    {
        public MyCourseDetailData data { get; set; }
    }

    public class MyCourseDetailData
    {
        public string courseID { get; set; }
        public string courseName { get; set; }
        public string secondCourseName { get; set; }
        public string courseDescription { get; set; }
        public string createBy { get; set; }
        public DateTime createDate { get; set; }
        public string linkCourseIntroductionVideo { get; set; }
        public string remark { get; set; }
        public List<MyCourseSection> courseSection { get; set; }
    }

    public class MyCourseSection
    {
        public string courseSectionID { get; set; }
        public int sectionNumber { get; set; }
        public string sectionName { get; set; }
        public List<MyCourseVideo> courseVideo { get; set; }
    }

    public class MyCourseVideo
    {
        public string videoName { get; set; }
        public int videoNumber { get; set; }
        public string videoLength { get; set; }
        public string linkCourseVideo { get; set; }
    }
}