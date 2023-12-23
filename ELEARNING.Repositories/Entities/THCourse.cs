using System;

namespace ELEARNING.Repositories.Entities
{
    public class THCourse
    {
        public Guid ID { get; set; }
        public string Course_Name { get; set; }
        public string Second_Course_Name { get; set; }
        public string Course_Desc { get; set; }
        public string Link_Cover_Course_Video_ID { get; set; }
        public string Video_ID { get; set; }
        public string Level_ID { get; set; }
        public decimal Price { get; set; }
        public string Remark { get; set; }
        public string Create_By { get; set; }
        public DateTime Create_Date { get; set; }
        public string Update_By { get; set; }
        public DateTime? Update_Date { get; set; }
    }
}