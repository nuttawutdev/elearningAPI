using System;

namespace ELEARNING.Repositories.Entities
{
    public class TDCourseSection
    {
        public Guid ID { get; set; }
        public Guid Course_ID { get; set; }
        public string Section_Name { get; set; }
        public int Section_Number { get; set; }
        public string Create_By { get; set; }
        public DateTime Create_Date { get; set; }
        public string Update_By { get; set; }
        public DateTime? Update_Date { get; set; }
    }
}