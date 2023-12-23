using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace ELEARNING.Services.Models.Request
{
    public class CreateCourseRequest
    {
        public string courseName { get; set; }
        public string secondCourseName { get; set; }
        public string description { get; set; }
        public decimal price { get; set; }
        public IFormFile coverImageVideo { get; set; }
        public IFormFile introductionCourseVideo { get; set; }
        public int levelID { get; set; }
        public string remark { get; set; }
        public List<SectionVideoRequest> sectionVideo { get; set; }
    }

    public class SectionVideoRequest
    {
        public string sectionName { get; set; }
        public int sectionNumber { get; set; }
        public List<VideoRequest> videoList { get; set; }
    }

    public class VideoRequest
    {
        public string videoName { get; set; }
        public IFormFile video { get; set; }
    }
}