using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace ELEARNING.Services.Models.Request
{
    public class SaveCourseRequest
    {
        public string courseID { get; set; }
        public string courseName { get; set; }
        public string secondCourseName { get; set; }
        public string description { get; set; }
        public decimal price { get; set; }
        public IFormFile? coverImageVideo { get; set; }
        public IFormFile? introductionCourseVideo { get; set; }
        public List<CourseSectionVideoRequest> sectionVideo { get; set; }
        public string userID { get; set; }
        public string remark { get; set; }
    }

    public class CourseSectionVideoRequest
    {
        public string courseSectionID { get; set; }
        public string sectionName { get; set; }
        public int sectionNumber { get; set; }
        public List<CourseVideoRequest> videoList { get; set; }
        public string action { get; set; }//add, edit, delete
    }

    public class CourseVideoRequest
    {
        public string courseVideoID { get; set; }
        public string videoName { get; set; }
        public IFormFile? video { get; set; }
        public string action { get; set; }
    }
}