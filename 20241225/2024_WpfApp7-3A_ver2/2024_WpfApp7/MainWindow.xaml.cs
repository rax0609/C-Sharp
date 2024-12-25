using LiveCharts;
using LiveCharts.Wpf;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace _2024_WpfApp7
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string aqiURL = "https://data.moenv.gov.tw/api/v2/aqx_p_432?api_key=e8dd42e6-9b8b-43f8-991e-b3dee723a52d&limit=1000&sort=ImportDate%20desc&format=JSON";

        AQIdata aqiData = new AQIdata();
        List<Field> fields = new List<Field>();
        List<Record> records = new List<Record>();
        SeriesCollection seriesCollection = new SeriesCollection();
        List<Record> selectedRecords = new List<Record>();

        public MainWindow()
        {
            InitializeComponent();
            urlTextBox.Text = aqiURL;
        }

        private async void btnGetAQI_Click(object sender, RoutedEventArgs e)
        {
            string url = urlTextBox.Text;
            ContentTextBox.Text = "抓取資料中...";
            statusBarText.Text = "抓取資料中...";

            string data = await GetAQIAsync(url);
            if (data == null)
            {
                ContentTextBox.Text = "資料抓取失敗";
                return;
            }

            ContentTextBox.Text = data;

            try
            {
                aqiData = JsonSerializer.Deserialize<AQIdata>(data) ?? new AQIdata();
                fields = aqiData.fields?.ToList() ?? new List<Field>();
                records = aqiData.records?.ToList() ?? new List<Record>();
                selectedRecords = records;
                statusBarText.Text = $"共有{records.Count}筆資料";
                DisplayAQIData();
            }
            catch (JsonException ex)
            {
                ContentTextBox.Text = "資料解析失敗";
                statusBarText.Text = $"資料解析失敗: {ex.Message}";
            }
        }

        private void DisplayAQIData()
        {
            RecordDataGrid.ItemsSource = records;

            if (records.Count == 0) return;

            Record record = records[0];
            DataWrapPanel.Children.Clear();

            foreach (Field field in fields)
            {
                var propertyInfo = record.GetType().GetProperty(field.id);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(record) as string;
                    if (double.TryParse(value, out double v))
                    {
                        CheckBox cb = new CheckBox
                        {
                            Content = field.info.label,
                            Tag = field.id,
                            Margin = new Thickness(3),
                            FontSize = 14,
                            FontWeight = FontWeights.Bold,
                            Width = 120
                        };
                        cb.Checked += UpdateChart;
                        cb.Unchecked += UpdateChart;
                        DataWrapPanel.Children.Add(cb);
                    }
                }
            }
        }

        private void UpdateChart(object sender, RoutedEventArgs e)
        {
            seriesCollection.Clear();

            foreach (CheckBox cb in DataWrapPanel.Children)
            {
                if (cb.IsChecked == true)
                {
                    List<string> labels = new List<string>();
                    string tag = cb.Tag as string;
                    ColumnSeries columnSeries = new ColumnSeries();
                    ChartValues<double> values = new ChartValues<double>();

                    foreach (Record r in selectedRecords)
                    {
                        var propertyInfo = r.GetType().GetProperty(tag);
                        if (propertyInfo != null)
                        {
                            var value = propertyInfo.GetValue(r) as string;
                            if (double.TryParse(value, out double v))
                            {
                                labels.Add(r.sitename);
                                values.Add(v);
                            }
                        }
                    }
                    columnSeries.Values = values;
                    columnSeries.Title = tag;
                    columnSeries.LabelPoint = point => $"{labels[(int)point.X]}:{point.Y.ToString()}";
                    seriesCollection.Add(columnSeries);
                }
            }
            AQIChart.Series = seriesCollection;
        }

        private async Task<string> GetAQIAsync(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        statusBarText.Text = $"Error: {response.StatusCode}";
                        return null;
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            statusBarText.Text = "Error: 接收到的資料為空";
                            return null;
                        }

                        // 檢查是否包含 API KEY 錯誤訊息
                        if (content.Contains("API KEY 不存在") || content.Contains("API KEY 已經到期"))
                        {
                            statusBarText.Text = "Error: API KEY 不存在或已經到期";
                            return null;
                        }

                        return content;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                statusBarText.Text = $"HTTP Request Error: {ex.Message}";
                return null;
            }
            catch (Exception ex)
            {
                statusBarText.Text = $"Error: {ex.Message}";
                return null;
            }
        }

        private void RecordDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void RecordDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedRecords = RecordDataGrid.SelectedItems.Cast<Record>().ToList();
            statusBarText.Text = $"共有{selectedRecords.Count}筆資料";
        }
    }
}


