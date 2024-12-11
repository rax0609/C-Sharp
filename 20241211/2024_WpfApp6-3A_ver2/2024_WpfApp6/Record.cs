namespace _2024_WpfApp6
{
    internal class Record
    {
        public Student SelectedStudent { get; set; }
        public Course SelectedCourse { get; set; }
        public bool Equals(Record record)
        {
            return (SelectedStudent.StudentId == record.SelectedStudent.StudentId && SelectedCourse.CourseName == record.SelectedCourse.CourseName);
        }
    }
}

