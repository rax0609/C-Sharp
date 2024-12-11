using System.Collections.ObjectModel;

namespace _2024_WpfApp6
{
    internal class Teacher
    {
        public String TeacherName { get; set; }

        public ObservableCollection<Course> TeachingCourses { get; set; }

        public Teacher(String TeacherName)
        {
            this.TeacherName = TeacherName;
            TeachingCourses = new ObservableCollection<Course>();
        }
        public override string ToString()
        {
            return TeacherName;
        }
    }
}
