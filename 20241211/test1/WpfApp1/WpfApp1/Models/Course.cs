namespace WpfApp1.Models
{
    public class Course
    {
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int Points { get; set; }
        public string OpeningClass { get; set; }
        public Teacher Teacher { get; set; }

        public Course(string courseId, string courseName, string description,
                     string type, int points, string openingClass, Teacher teacher)
        {
            CourseId = courseId;
            CourseName = courseName;
            Description = description;
            Type = type;
            Points = points;
            OpeningClass = openingClass;
            Teacher = teacher;
        }
    }
}
