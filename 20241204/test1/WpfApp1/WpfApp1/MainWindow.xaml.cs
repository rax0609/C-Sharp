using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private List<Student> students;
        private List<Teacher> teachers;
        private ObservableCollection<Course> selectedCourses;
        private int totalCredits = 0;
        private const int MAX_CREDITS = 25;

        public MainWindow()
        {
            InitializeComponent();
            students = new List<Student>();
            teachers = new List<Teacher>();
            selectedCourses = new ObservableCollection<Course>();

            InitializeData();
            InitializeUI();
            SetupEventHandlers();
        }

        private void InitializeData()
        {
            // 新增學生資料
            students.Add(new Student("S001", "陳小明"));
            students.Add(new Student("S002", "林小華"));
            students.Add(new Student("S003", "張小英"));
            students.Add(new Student("S004", "王小強"));

            // 新增老師資料
            var teacher1 = new Teacher("陳定宏");
            var teacher2 = new Teacher("張鴻德");
            var teacher3 = new Teacher("洪國鈞");

            // 新增課程到老師的教學課程清單
            teacher1.TeachingCourses.Add(new Course(
                "C001",
                "視窗程式設計",
                "本課程使用WPF類別庫和C#語言來設計桌面視窗應用程式",
                "必修",
                6,
                "五專資工三甲",
                teacher1
            ));

            teacher1.TeachingCourses.Add(new Course(
                "C002",
                "視窗程式設計",
                "本課程使用WPF類別庫和C#語言來設計桌面視窗應用程式",
                "選修",
                3,
                "四技資工二甲",
                teacher1
            ));

            teacher2.TeachingCourses.Add(new Course(
                "C004",
                "網頁程式設計",
                "本課程使用HTML5、CSS3、JavaScript等技術來設計網頁程式",
                "必修",
                6,
                "五專資工三甲",
                teacher2
            ));

            teacher3.TeachingCourses.Add(new Course(
                "C006",
                "資料庫程式設計",
                "本課程使用SQL Server資料庫和C#語言來設計資料庫應用程式",
                "必修",
                6,
                "五專資工三甲",
                teacher3
            ));

            teachers.AddRange(new[] { teacher1, teacher2, teacher3 });
        }

        private void InitializeUI()
        {
            // 設定學生下拉選單
            cmbStudent.ItemsSource = students;
            cmbStudent.DisplayMemberPath = "StudentName";
            cmbStudent.SelectedValuePath = "StudentId";

            // 設定教師TreeView
            tvTeacher.ItemsSource = teachers;

            // 設定已選課程ListView
            lvSelectedCourses.ItemsSource = selectedCourses;

            // 初始化狀態
            UpdateStatus("系統就緒");
            UpdateCredits();
        }

        private void SetupEventHandlers()
        {
            // 設定按鈕事件
            btnAdd.Click += BtnAdd_Click;
            btnDelete.Click += BtnDelete_Click;
            btnSave.Click += BtnSave_Click;

            // 設定選擇變更事件
            cmbStudent.SelectionChanged += CmbStudent_SelectionChanged;
            tvTeacher.SelectedItemChanged += TvTeacher_SelectedItemChanged;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var selectedCourse = tvTeacher.SelectedItem as Course;
            if (selectedCourse == null)
            {
                UpdateStatus("請先選擇要加選的課程");
                return;
            }

            if (totalCredits + selectedCourse.Points > MAX_CREDITS)
            {
                MessageBox.Show("超過學分上限！", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!selectedCourses.Any(c => c.CourseId == selectedCourse.CourseId))
            {
                selectedCourses.Add(selectedCourse);
                totalCredits += selectedCourse.Points;
                UpdateCredits();
                UpdateStatus($"已加選 {selectedCourse.CourseName}");
            }
            else
            {
                UpdateStatus("該課程已在選課清單中");
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedCourse = lvSelectedCourses.SelectedItem as Course;
            if (selectedCourse != null)
            {
                selectedCourses.Remove(selectedCourse);
                totalCredits -= selectedCourse.Points;
                UpdateCredits();
                UpdateStatus($"已退選 {selectedCourse.CourseName}");
            }
            else
            {
                UpdateStatus("請先選擇要退選的課程");
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cmbStudent.SelectedItem == null)
            {
                UpdateStatus("請先選擇學生");
                return;
            }

            var student = cmbStudent.SelectedItem as Student;
            MessageBox.Show($"已儲存 {student.StudentName} 的選課記錄！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            UpdateStatus($"已儲存 {student.StudentName} 的選課記錄");
        }

        private void CmbStudent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var student = cmbStudent.SelectedItem as Student;
            if (student != null)
            {
                selectedCourses.Clear();
                totalCredits = 0;
                UpdateCredits();
                UpdateStatus($"目前選課學生：{student.StudentName}");
            }
        }

        private void TvTeacher_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is Course course)
            {
                UpdateStatus($"已選擇課程：{course.CourseName}");
            }
        }

        private void UpdateStatus(string message)
        {
            txtStatus.Text = message;
        }

        private void UpdateCredits()
        {
            txtCredits.Text = totalCredits.ToString();
        }
    }
}
