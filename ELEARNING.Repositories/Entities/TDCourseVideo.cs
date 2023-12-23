using System;

namespace ELEARNING.Repositories.Entities
{
    public class TDCourseVideo
    {
        public Guid ID { get; set; }
        public Guid Course_Section_ID { get; set; }
        public Guid Course_ID { get; set; }
        public string Video_Name { get; set; }
        public string Video_ID { get; set; }
        public string Create_By { get; set; }
        public DateTime Create_Date { get; set; }
        public string Update_By { get; set; }
        public DateTime? Update_Date { get; set; }
    }
}