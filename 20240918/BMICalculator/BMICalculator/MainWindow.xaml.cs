using System;
using System.Windows;
using System.Windows.Media;

namespace BMICalculator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // 可以添加一些初始化代碼，如設定預設值等
        }

        private void CalculateBMI_Click(object sender, RoutedEventArgs e)
        {
            // 取得使用者輸入
            if (double.TryParse(HeightTextBox.Text, out double heightCm) && double.TryParse(WeightTextBox.Text, out double weightKg))
            {
                if (heightCm <= 0 || weightKg <= 0)
                {
                    MessageBox.Show("請輸入有效的身高和體重數值。", "輸入錯誤", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                double heightM = heightCm / 100;
                double bmi = weightKg / (heightM * heightM);
                string bmiCategory = GetBMICategory(bmi);

                ResultTextBlock.Text = $"您的 BMI 為 {bmi:F2} ({bmiCategory})";
            }
            else
            {
                MessageBox.Show("請確保身高和體重都是數字。", "輸入錯誤", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private string GetBMICategory(double bmi)
        {
            if (bmi < 18.5)
                return "體重過輕";
            else if (bmi < 24.9)
                return "正常範圍";
            else if (bmi < 29.9)
                return "過重";
            else
                return "肥胖";
        }
    }
}
