using System;

namespace ELEARNING.Services.Models.Request
{
    public class GetMyCourseDetailRequest
    {
        public string userID { get; set; }
        public string courseID { get; set; }
    }
}