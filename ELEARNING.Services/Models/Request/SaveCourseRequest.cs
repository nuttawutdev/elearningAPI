using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace ELEARNING.Services.Models.Request
{
    public class SaveCourseRequest
    {
        public Guid? courseID { get; set; }
        public string courseName { get; set; }
        public string secondCourseName { get; set; }
        public string description { get; set; }
        public decimal price { get; set; }
        public IFormFile? coverImageVideo { get; set; }
        public IFormFile? introductionCourseVideo { get; set; }
        public int levelID { get; set; }
        public string remark { get; set; }
        public List<CourseSectionVideoRequest> sectionVideo { get; set; }
    }

    public class CourseSectionVideoRequest
    {
        public Guid? courseSectionID { get; set; }
        public string sectionName { get; set; }
        public int sectionNumber { get; set; }
        public List<CourseVideoRequest> videoList { get; set; }
        public string action { get; set; }
    }

    public class CourseVideoRequest
    {
        public Guid? courseVideoID { get; set; }
        public string videoName { get; set; }
        public IFormFile? video { get; set; }
        public string action { get; set; }
    }
}