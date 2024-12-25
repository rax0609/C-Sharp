using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace _2024_WpfApp6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Student> students = new List<Student>();
        List<Course> courses = new List<Course>();
        List<Teacher> teachers = new List<Teacher>();
        List<Record> records = new List<Record>();

        Student selectedStudent = null;
        Course selectedCourse = null;
        Teacher selectedTeacher = null;
        Record selectedRecord = null;
        public MainWindow()
        {
            InitializeComponent();
            InitializeData();

            // 將學生資料填入cmbStudent
            cmbStudent.ItemsSource = students;
            cmbStudent.SelectedIndex = 0;

            // 將以教師分類之課程填入tvTeacher
            tvTeacher.ItemsSource = teachers;
        }

        private void InitializeData()
        {
            // 新增學生資料
            students.Add(new Student { StudentId = "S001", StudentName = "陳小明" });
            students.Add(new Student { StudentId = "S002", StudentName = "林小華" });
            students.Add(new Student { StudentId = "S003", StudentName = "張小英" });
            students.Add(new Student { StudentId = "S004", StudentName = "王小強" });

            // 新增老師資料
            Teacher teacher1 = new Teacher("陳定宏");
            teacher1.TeachingCourses.Add(new Course(teacher1) { CourseId = "C001", CourseName = "視窗程式設計", CourseDescription = "本課程使用WPF類別庫和C#語言來設計桌面視窗應用程式", Type = "必修", Points = 6, OpeningClass = "五專資工三甲" });
            teacher1.TeachingCourses.Add(new Course(teacher1) { CourseId = "C002", CourseName = "視窗程式設計", CourseDescription = "本課程使用WPF類別庫和C#語言來設計桌面視窗應用程式", Type = "選修", Points = 3, OpeningClass = "四技資工二甲" });
            teacher1.TeachingCourses.Add(new Course(teacher1) { CourseId = "C003", CourseName = "計算機程式", CourseDescription = "程式設計是資訊工程學生的基礎課程，本課程主要希望帶領學生能夠瞭解並開始學習程式設計，此課成主要教授以C語言為主。", Type = "必修", Points = 2, OpeningClass = "四技資工一丙" });

            Teacher teacher2 = new Teacher("張鴻德");
            teacher2.TeachingCourses.Add(new Course(teacher2) { CourseId = "C004", CourseName = "網頁程式設計", CourseDescription = "本課程使用HTML5、CSS3、JavaScript、jQuery、Bootstrap等技術來設計網頁程式", Type = "必修", Points = 6, OpeningClass = "五專資工三甲" });
            teacher2.TeachingCourses.Add(new Course(teacher2) { CourseId = "C005", CourseName = "網頁程式設計", CourseDescription = "本課程使用HTML5、CSS3、JavaScript、jQuery、Bootstrap等技術來設計網頁程式", Type = "選修", Points = 3, OpeningClass = "四技資工二甲" });

            Teacher teacher3 = new Teacher("洪國鈞");
            teacher3.TeachingCourses.Add(new Course(teacher3) { CourseId = "C006", CourseName = "資料庫程式設計", CourseDescription = "本課程使用SQL Server資料庫和C#語言來設計資料庫應用程式", Type = "必修", Points = 6, OpeningClass = "五專資工三甲" });
            teacher3.TeachingCourses.Add(new Course(teacher3) { CourseId = "C007", CourseName = "智慧型系統應用", CourseDescription = "本課程完整而淺顯地介紹研習人工智慧技術、智慧型系統與相關機電資領域所需的專業基礎，並詳細探討各種新進的智慧型系統應用技術。", Type = "選修", Points = 3, OpeningClass = "四技控晶四甲, 四技控晶四乙" });
            teachers.AddRange(new Teacher[] { teacher1, teacher2, teacher3 });

            foreach (Teacher teacher in teachers)
            {
                foreach (Course course in teacher.TeachingCourses)
                {
                    courses.Add(course);
                }
            }
            lbCourse.ItemsSource = courses;
        }

        private void cmbStudent_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            selectedStudent = cmbStudent.SelectedItem as Student;
            labelStatus.Content = $"選擇學生：{selectedStudent.StudentName}";
        }

        private void tvTeacher_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (tvTeacher.SelectedItem is Course)
            {
                selectedCourse = tvTeacher.SelectedItem as Course;
                labelStatus.Content = $"選擇課程：{selectedCourse.CourseName}";
            }
            else if (tvTeacher.SelectedItem is Teacher)
            {
                selectedTeacher = tvTeacher.SelectedItem as Teacher;
                labelStatus.Content = $"選擇教師：{selectedTeacher.TeacherName}";
            }
        }

        private void lbCourse_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            selectedCourse = lbCourse.SelectedItem as Course;
            labelStatus.Content = $"選擇課程：{selectedCourse.CourseName}";
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (selectedStudent == null || selectedCourse == null)
            {
                MessageBox.Show("請選取學生或課程");
                return;
            }
            else
            {
                Record newRecord = new Record
                {
                    SelectedStudent = selectedStudent,
                    SelectedCourse = selectedCourse
                };

                foreach (Record r in records)
                {
                    if (r.Equals(newRecord))
                    {
                        MessageBox.Show("此學生已選取此課程");
                        return;
                    }
                }
                records.Add(newRecord);
                lvRecord.ItemsSource = records;
                lvRecord.Items.Refresh();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRecord == null)
            {
                MessageBox.Show("請選取紀錄");
                return;
            }
            else
            {
                records.Remove(selectedRecord);
                lvRecord.ItemsSource = records;
                lvRecord.Items.Refresh();
            }
        }

        private void lvRecord_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (lvRecord.SelectedItem is Record)
            {
                selectedRecord = lvRecord.SelectedItem as Record;
                labelStatus.Content = $"選擇紀錄：{selectedRecord.SelectedStudent.StudentName} - {selectedRecord.SelectedCourse.CourseName}";
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Json Files(*.json)|*.json|All Files(*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                string json = JsonSerializer.Serialize(records, options);
                File.WriteAllText(saveFileDialog.FileName, json);
                MessageBox.Show("資料已儲存");
            }
        }
    }
}