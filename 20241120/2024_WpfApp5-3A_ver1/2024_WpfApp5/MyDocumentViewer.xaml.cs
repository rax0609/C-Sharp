using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using OpenXmlRun = DocumentFormat.OpenXml.Wordprocessing.Run;

// 添加别名指令
using WpfParagraph = System.Windows.Documents.Paragraph;
using WpfRun = System.Windows.Documents.Run;
using OpenXmlWordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;

namespace _2024_WpfApp5
{
    public partial class MyDocumentViewer : Window
    {
        private string currentFilePath = string.Empty;

        public MyDocumentViewer()
        {
            InitializeComponent();
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "打开文件",
                Filter = "所有支持的文件|*.docx;*.txt;*.pdf|Word Documents (*.docx)|*.docx|PDF Files (*.pdf)|*.pdf|Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                currentFilePath = openFileDialog.FileName;
                string extension = Path.GetExtension(currentFilePath).ToLower();

                switch (extension)
                {
                    case ".pdf":
                        OpenPdfFile(currentFilePath);
                        break;
                    case ".docx":
                        OpenDocxFile(currentFilePath);
                        break;
                    default:
                        OpenTextFile(currentFilePath);
                        break;
                }
            }
        }

        private void OpenPdfFile(string filePath)
        {
            try
            {
                // 清空 RichTextBox
                DocumentRichTextBox.Document.Blocks.Clear();

                using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(filePath)))
                {
                    for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                    {
                        // 创建文本提取策略
                        var strategy = new LocationTextExtractionStrategy();

                        // 提取当前页的文本
                        string pageText = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i), strategy);

                        // 为每一页创建新的段落
                        WpfParagraph paragraph = new WpfParagraph();

                        // 添加页码标记（修复了 Bold 的使用方式）
                        WpfRun pageNumberRun = new WpfRun();
                        pageNumberRun.Text = $"Page {i}\n";
                        pageNumberRun.FontWeight = System.Windows.FontWeights.Bold;
                        paragraph.Inlines.Add(pageNumberRun);

                        // 添加页面内容
                        WpfRun contentRun = new WpfRun();
                        contentRun.Text = pageText;
                        paragraph.Inlines.Add(contentRun);

                        // 添加段落到文档
                        DocumentRichTextBox.Document.Blocks.Add(paragraph);

                        // 在页面之间添加分隔符（可选）
                        if (i < pdfDoc.GetNumberOfPages())
                        {
                            WpfParagraph separator = new WpfParagraph();
                            WpfRun separatorRun = new WpfRun();
                            separatorRun.Text = "----------------------------------------";
                            separator.Inlines.Add(separatorRun);
                            DocumentRichTextBox.Document.Blocks.Add(separator);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法打开 PDF 文件。错误: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenDocxFile(string filePath)
        {
            try
            {
                // 清空 RichTextBox
                DocumentRichTextBox.Document.Blocks.Clear();

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
                {
                    Body body = wordDoc.MainDocumentPart.Document.Body;
                    if (body != null)
                    {
                        foreach (var element in body.Elements())
                        {
                            if (element is OpenXmlWordParagraph openXmlParagraph)
                            {
                                WpfParagraph wpfParagraph = new WpfParagraph();

                                foreach (var run in openXmlParagraph.Elements<OpenXmlRun>())
                                {
                                    WpfRun wpfRun = new WpfRun();

                                    foreach (var text in run.Elements<Text>())
                                    {
                                        if (text != null)
                                        {
                                            wpfRun.Text += text.Text;
                                        }
                                    }

                                    wpfParagraph.Inlines.Add(wpfRun);
                                }

                                DocumentRichTextBox.Document.Blocks.Add(wpfParagraph);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法打开 .docx 文件。错误: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenTextFile(string filePath)
        {
            try
            {
                string fileContent = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
                DocumentRichTextBox.Document.Blocks.Clear();
                DocumentRichTextBox.AppendText(fileContent);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法打开文件。错误: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 保存文件按钮的事件处理程序
        /// </summary>
        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "保存文件",
                Filter = "Word Documents (*.docx)|*.docx|Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                FileName = Path.GetFileName(currentFilePath)
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                currentFilePath = saveFileDialog.FileName;
                string extension = Path.GetExtension(currentFilePath).ToLower();

                if (extension == ".docx")
                {
                    try
                    {
                        // 创建新的 docx 文件
                        using (WordprocessingDocument wordDoc = WordprocessingDocument.Create(currentFilePath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
                        {
                            MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
                            mainPart.Document = new Document();
                            Body body = mainPart.Document.AppendChild(new Body());

                            // 从 RichTextBox 中提取文本并写入 docx
                            foreach (var block in DocumentRichTextBox.Document.Blocks)
                            {
                                if (block is WpfParagraph wpfParagraph)
                                {
                                    OpenXmlWordParagraph openXmlParagraph = new OpenXmlWordParagraph();

                                    foreach (var inline in wpfParagraph.Inlines)
                                    {
                                        if (inline is WpfRun run)
                                        {
                                            OpenXmlRun openXmlRun = new OpenXmlRun();
                                            openXmlRun.AppendChild(new Text(run.Text ?? string.Empty));
                                            openXmlParagraph.AppendChild(openXmlRun);
                                        }
                                    }

                                    body.AppendChild(openXmlParagraph);
                                }
                            }
                        }

                        MessageBox.Show("文件保存成功。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"无法保存 .docx 文件。错误: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    try
                    {
                        TextRange range = new TextRange(DocumentRichTextBox.Document.ContentStart, DocumentRichTextBox.Document.ContentEnd);
                        File.WriteAllText(currentFilePath, range.Text, System.Text.Encoding.UTF8);
                        MessageBox.Show("文件保存成功。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"无法保存文件。错误: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        /// <summary>
        /// 关闭按钮的事件处理程序
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
