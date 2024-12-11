using System.Collections.ObjectModel;

namespace WpfApp1.Models
{
    public class Teacher
    {
        public string TeacherId { get; set; }
        public string TeacherName { get; set; }
        public ObservableCollection<Course> TeachingCourses { get; set; }

        public Teacher(string teacherName)
        {
            TeacherId = System.Guid.NewGuid().ToString();
            TeacherName = teacherName;
            TeachingCourses = new ObservableCollection<Course>();
        }
    }
}
