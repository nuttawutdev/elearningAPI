using System;
using System.Collections.Generic;
using ELEARNING.Services.Models.Response;
using Microsoft.AspNetCore.Http;

namespace ELEARNING.Services.Models.Request
{
    public class CreateCourseResponse : BaseResponse
    {
        public CreateCourseResult data { get; set; }
    }

    public class CreateCourseResult
    {
        public Guid courseID { get; set; }
    }
}