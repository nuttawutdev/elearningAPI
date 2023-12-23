using System;
using System.Collections.Generic;

namespace ELEARNING.Services.Models.Response
{
    public class GetAllVideoResponse : BaseResponse
    {
        public List<VideoData> data { get; set; }
    }

    public class VideoData
    {
        public Guid courseID { get; set; }
        public string courseName { get; set; }
        public string createBy { get; set; }
        public decimal price { get; set; }
        public string linkCoverCourseVideo { get; set; }
    }
}