namespace WpfApp1.Models
{
    public class Student
    {
        public string StudentId { get; set; }
        public string StudentName { get; set; }

        public Student(string studentId, string studentName)
        {
            StudentId = studentId;
            StudentName = studentName;
        }
    }
}
