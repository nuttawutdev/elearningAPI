namespace ELEARNING.Services.Models.Request
{
    public class GetAllMyCourseRequest
    {
        public int pageNo { get; set; }
        public int pageSize { get; set; }
        public string userID { get; set; }
    }
}