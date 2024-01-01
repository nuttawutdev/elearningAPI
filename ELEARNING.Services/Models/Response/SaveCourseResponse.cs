using System;
using System.Collections.Generic;
using ELEARNING.Services.Models.Response;
using Microsoft.AspNetCore.Http;

namespace ELEARNING.Services.Models.Request
{
    public class SaveCourseResponse : BaseResponse
    {
        public SaveCourseResult data { get; set; }
    }

    public class SaveCourseResult
    {
        public string courseID { get; set; }
    }
}