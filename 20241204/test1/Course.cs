namespace WpfApp1
{
    internal class Course
    {
        public String CourseId { get; set; }
        public String CourseName { get; set; }
        public String CourseDescription { get; set; }   
        public String Type { get; set; }
        public int Points { get; set; }
        public String OpeningClass { get; set; }
        public Teacher Tutor{ get; set; }
        public Course(Teacher tutor)
        {
            Tutor = tutor;
        }
    }
}