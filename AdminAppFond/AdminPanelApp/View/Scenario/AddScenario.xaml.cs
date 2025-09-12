using AdminPanelApp.Logic;
using AdminPanelApp.Models.Scenario;
using AdminPanelApp.Requests;
using ETS.Resources;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static DevExpress.Utils.Filtering.ExcelFilterOptions;

namespace AdminPanelApp.View.Scenario
{
    /// <summary>
    /// Логика взаимодействия для AddScenario.xaml
    /// </summary>
    public partial class AddScenario
    {
        private TgMenuModel _tgMenuModel;

        public AddScenario(TgMenuModel tgMenuModel)
        {
            InitializeComponent();
            _tgMenuModel = tgMenuModel;
            LoadTgMenuModelData();

            TxbxText.PreviewMouseRightButtonUp += RichTextBox_PreviewMouseRightButtonUp;
        }



        private void LoadTgMenuModelData()
        {
            if (_tgMenuModel == null)
                return;

            TxbxName.Text = _tgMenuModel.Name ?? string.Empty;
            //TxbxType.Text = _tgMenuModel.Type ?? string.Empty;
            //TxbxNameMenu.Text = _tgMenuModel.NameMenu ?? string.Empty;
            //TxbxIcon.Text = _tgMenuModel.Icon ?? string.Empty;
            TxbxColumn.Text = _tgMenuModel.Column.ToString();
            TxbxRow.Text = _tgMenuModel.Row.ToString();
            LoadFromString(_tgMenuModel.Text);

            List<string> nameMenu = new List<string>();
            nameMenu.Add("Главное меню");

            foreach (var item in LogicData.TgMenus)
            {
                if (item != _tgMenuModel)
                { 
                    nameMenu.Add(item.Name);
                }
            }

            CmbxMainMenu.ItemsSource = nameMenu;
            CmbxMainMenu.SelectedIndex = 0;
            if (!String.IsNullOrEmpty(_tgMenuModel.Level))
                CmbxMainMenu.SelectedItem = _tgMenuModel.Level;

            TxbxPathToDocument.Text = _tgMenuModel.PathToDocument;
            //TxbxPathToDocument.Text = _tgMenuModel.PathToDocument ?? string.Empty;
        }

        // Загрузить из строки (автоопределение формата)
        private void LoadFromString(string text)
        {
            if (String.IsNullOrEmpty(text))
                return;

            if (text.Contains("<Section") && text.Contains("xmlns="))
            {
                // XAML формат
                RichTextBoxLoader.LoadFromXaml(TxbxText, text);
            }
            else if (text.Contains("<") && text.Contains(">"))
            {
                // HTML формат
                RichTextBoxLoader.LoadFromHtml(TxbxText, text);
            }
            else
            {
                // Простой текст
                RichTextBoxLoader.LoadPlainText(TxbxText, text);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(TxbxName.Text))
                return;

            if (String.IsNullOrEmpty(_tgMenuModel.Name))
            {
                _tgMenuModel.IsRun = true;
                LogicData.TgMenus.Add(_tgMenuModel);
            }

            _tgMenuModel.PathToDocument = TxbxPathToDocument.Text;
            _tgMenuModel.Name = TxbxName.Text.Trim();
            //_tgMenuModel.Icon = string.IsNullOrWhiteSpace(TxbxIcon.Text) ? null : TxbxIcon.Text.Trim();

            _tgMenuModel.Column = string.IsNullOrWhiteSpace(TxbxColumn.Text) ? 0 : Convert.ToInt32(TxbxColumn.Text.Trim());
            _tgMenuModel.Row = string.IsNullOrWhiteSpace(TxbxRow.Text) ? 0 : Convert.ToInt32(TxbxRow.Text.Trim());

            _tgMenuModel.Text = SimpleTelegramFormatter.ToTelegramHtml(TxbxText);
            _tgMenuModel.Level = CmbxMainMenu.SelectedItem.ToString();
            //_tgMenuModel.PathToDocument = string.IsNullOrWhiteSpace(TxbxPathToDocument.Text) ? null : TxbxPathToDocument.Text.Trim();

            if (_tgMenuModel.Id == 0)
                TgMenuRequests.AddTgMenu(_tgMenuModel);
            else
                TgMenuRequests.UpdateTgMenu(_tgMenuModel);

                DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void RichTextBox_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Обновляем состояние меню перед показом
            UpdateMenuItemsState();
        }

        private void UpdateMenuItemsState()
        {
            // Проверяем, есть ли выделение
            bool hasSelection = !TxbxText.Selection.IsEmpty;

            // Находим элементы меню и обновляем их состояние
            var contextMenu = TxbxText.ContextMenu;
            if (contextMenu != null)
            {
                foreach (var item in contextMenu.Items)
                {
                    if (item is MenuItem menuItem)
                    {
                        switch (menuItem.Header)
                        {
                            case "Жирный":
                            case "Курсив":
                                menuItem.IsEnabled = hasSelection;
                                break;
                            case "Добавить ссылку":
                                menuItem.IsEnabled = hasSelection;
                                break;
                        }
                    }
                }
            }
        }

        private void MakeBold_Click(object sender, RoutedEventArgs e)
        {
            if (TxbxText.Selection.IsEmpty)
                return;

            var currentWeight = TxbxText.Selection.GetPropertyValue(TextElement.FontWeightProperty);
            var newWeight = currentWeight.Equals(FontWeights.Bold) ?
                FontWeights.Normal : FontWeights.Bold;

            TxbxText.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, newWeight);
        }

        private void MakeItalic_Click(object sender, RoutedEventArgs e)
        {
            if (TxbxText.Selection.IsEmpty)
                return;

            var currentStyle = TxbxText.Selection.GetPropertyValue(TextElement.FontStyleProperty);
            var newStyle = currentStyle.Equals(FontStyles.Italic) ?
                FontStyles.Normal : FontStyles.Italic;

            TxbxText.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, newStyle);
        }

        private void AddHyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (TxbxText.Selection.IsEmpty)
            {
                MessageBox.Show("Выделите текст для создания ссылки");
                return;
            }

            // Диалог для ввода URL
            var dialog = new HyperlinkDialog();
            if (dialog.ShowDialog() == true && !string.IsNullOrEmpty(dialog.Url))
            {
                CreateHyperlink(dialog.Url);
            }
        }

        private void CreateHyperlink(string url)
        {
            try
            {
                // Сохраняем выделенный текст
                string selectedText = new TextRange(
                    TxbxText.Selection.Start,
                    TxbxText.Selection.End).Text;

                // Создаем гиперссылку
                var hyperlink = new Hyperlink(TxbxText.Selection.Start, TxbxText.Selection.End)
                {
                    NavigateUri = new Uri(url.StartsWith("http") ? url : "https://" + url),
                    ToolTip = url
                };

                // Устанавливаем стиль для гиперссылки (синий с подчеркиванием)
                //hyperlink.Foreground = Brushes.Blue;
                hyperlink.TextDecorations = TextDecorations.Underline;

                hyperlink.RequestNavigate += (s, e) =>
                {
                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = e.Uri.ToString(),
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка открытия ссылки: {ex.Message}");
                    }
                };
            }
            catch (UriFormatException)
            {
                MessageBox.Show("Некорректный URL адрес");
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            TxbxText.Copy();
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            TxbxText.Paste();
        }

        public static class SimpleTelegramFormatter
        {
            public static string ToTelegramHtml(RichTextBox richTextBox)
            {
                var result = new StringBuilder();
                var document = richTextBox.Document;

                foreach (var block in document.Blocks)
                {
                    if (block is Paragraph paragraph)
                    {
                        ProcessInlines(paragraph.Inlines, result);
                        result.Append("\n"); // Новый абзац
                    }
                }

                return result.ToString().Trim();
            }

            private static void ProcessInlines(InlineCollection inlines, StringBuilder sb)
            {
                foreach (var inline in inlines)
                {
                    if (inline is Run run)
                    {
                        ProcessRun(run, sb);
                    }
                    else if (inline is Hyperlink hyperlink)
                    {
                        ProcessHyperlink(hyperlink, sb);
                    }
                    else if (inline is Span span)
                    {
                        ProcessInlines(span.Inlines, sb);
                    }
                }
            }

            private static void ProcessRun(Run run, StringBuilder sb)
            {
                string text = EscapeHtml(run.Text);
                bool isBold = run.FontWeight == FontWeights.Bold;
                bool isItalic = run.FontStyle == FontStyles.Italic;
                bool isUnderline = run.TextDecorations != null &&
                                  run.TextDecorations.Contains(TextDecorations.Underline[0]);

                if (isBold) sb.Append("<b>");
                if (isItalic) sb.Append("<i>");
                if (isUnderline) sb.Append("<u>");

                sb.Append(text);

                if (isUnderline) sb.Append("</u>");
                if (isItalic) sb.Append("</i>");
                if (isBold) sb.Append("</b>");
            }

            private static void ProcessHyperlink(Hyperlink hyperlink, StringBuilder sb)
            {
                string url = hyperlink.NavigateUri?.ToString();
                string text = new TextRange(hyperlink.ContentStart, hyperlink.ContentEnd).Text;

                if (!string.IsNullOrEmpty(url))
                {
                    sb.Append($"<a href=\"{EscapeHtml(url)}\">{EscapeHtml(text)}</a>");
                }
                else
                {
                    sb.Append(EscapeHtml(text));
                }
            }

            private static string EscapeHtml(string text)
            {
                return text
                    .Replace("&", "&amp;")
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;");
            }
        }



        public partial class HyperlinkDialog : Window
        {
            public string Url { get; private set; }

            public HyperlinkDialog()
            {
                InitializeComponent();
                Width = 300;
                Height = 150;
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            private void InitializeComponent()
            {
                var stackPanel = new StackPanel { Margin = new Thickness(10) };

                var textBlock = new TextBlock
                {
                    Text = "Введите URL:",
                    Margin = new Thickness(0, 0, 0, 10)
                };

                var textBox = new TextBox
                {
                    Name = "urlTextBox",
                    Margin = new Thickness(0, 0, 0, 10)
                };

                var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };

                var okButton = new Button
                {
                    Content = "OK",
                    Width = 75,
                    Margin = new Thickness(0, 0, 10, 0),
                    IsDefault = true
                };

                var cancelButton = new Button
                {
                    Content = "Отмена",
                    Width = 75,
                    IsCancel = true
                };

                okButton.Click += (s, e) =>
                {
                    Url = textBox.Text;
                    DialogResult = true;
                    Close();
                };

                cancelButton.Click += (s, e) =>
                {
                    DialogResult = false;
                    Close();
                };

                buttonPanel.Children.Add(okButton);
                buttonPanel.Children.Add(cancelButton);

                stackPanel.Children.Add(textBlock);
                stackPanel.Children.Add(textBox);
                stackPanel.Children.Add(buttonPanel);

                Content = stackPanel;
            }
        }


        public static class RichTextBoxLoader
        {
            // Загрузка из XAML строки
            public static void LoadFromXaml(RichTextBox richTextBox, string xamlText)
            {
                if (string.IsNullOrEmpty(xamlText))
                {
                    ClearRichTextBox(richTextBox);
                    return;
                }

                try
                {
                    using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xamlText)))
                    {
                        TextRange textRange = new TextRange(
                            richTextBox.Document.ContentStart,
                            richTextBox.Document.ContentEnd);

                        textRange.Load(stream, DataFormats.Xaml);
                    }
                }
                catch (Exception ex)
                {
                    // Если XAML некорректный, загружаем как plain text
                    LoadPlainText(richTextBox, xamlText);
                    Console.WriteLine($"Ошибка загрузки XAML: {ex.Message}");
                }
            }

            // Загрузка из HTML (для Telegram формата)
            public static void LoadFromHtml(RichTextBox richTextBox, string htmlText)
            {
                if (string.IsNullOrEmpty(htmlText))
                {
                    ClearRichTextBox(richTextBox);
                    return;
                }

                try
                {
                    // Конвертируем HTML в XAML
                    string xamlText = ConvertHtmlToXaml(htmlText);
                    LoadFromXaml(richTextBox, xamlText);
                }
                catch (Exception ex)
                {
                    LoadPlainText(richTextBox, htmlText);
                    Console.WriteLine($"Ошибка загрузки HTML: {ex.Message}");
                }
            }

            // Загрузка простого текста
            public static void LoadPlainText(RichTextBox richTextBox, string plainText)
            {
                ClearRichTextBox(richTextBox);

                if (!string.IsNullOrEmpty(plainText))
                {
                    Paragraph paragraph = new Paragraph(new Run(plainText));
                    richTextBox.Document.Blocks.Add(paragraph);
                }
            }

            // Очистка RichTextBox
            private static void ClearRichTextBox(RichTextBox richTextBox)
            {
                richTextBox.Document.Blocks.Clear();
            }

            // Простой конвертер HTML в XAML (базовые теги)
            private static string ConvertHtmlToXaml(string html)
            {
                // Обрабатываем переносы строк и абзацы
                html = html
                    .Replace("\n", "</Paragraph><Paragraph>")  // Переносы строк
                    .Replace("<br>", "</Paragraph><Paragraph>") // HTML переносы
                    .Replace("<br/>", "</Paragraph><Paragraph>")
                    .Replace("<br />", "</Paragraph><Paragraph>")
                    .Replace("<p>", "</Paragraph><Paragraph>")  // Абзацы
                    .Replace("</p>", "</Paragraph><Paragraph>");

                // Упрощенная конвертация основных тегов Telegram
                var xaml = html
                    .Replace("<b>", "<Bold>")
                    .Replace("</b>", "</Bold>")
                    .Replace("<strong>", "<Bold>")
                    .Replace("</strong>", "</Bold>")
                    .Replace("<i>", "<Italic>")
                    .Replace("</i>", "</Italic>")
                    .Replace("<em>", "<Italic>")
                    .Replace("</em>", "</Italic>")
                    .Replace("<u>", "<Underline>")
                    .Replace("</u>", "</Underline>")
                    .Replace("<code>", "<Span FontFamily=\"Courier New\">")
                    .Replace("</code>", "</Span>")
                    .Replace("<pre>", "<Span FontFamily=\"Courier New\">")
                    .Replace("</pre>", "</Span>")
                    .Replace("<a href=\"", "<Hyperlink NavigateUri=\"")
                    .Replace("</a>", "</Hyperlink>")
                    .Replace("\">", "\">");

                // Экранирование HTML entities
                xaml = xaml
                    .Replace("&amp;", "&")
                    .Replace("&lt;", "<")
                    .Replace("&gt;", ">")
                    .Replace("&quot;", "\"")
                    .Replace("&#39;", "'");

                // Базовая XAML структура
                return $"<Section xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Paragraph>{xaml}</Paragraph></Section>";
            }
        }

        private void BtnPathToDocument_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();

            if (file.ShowDialog() == true)
            {
                TxbxPathToDocument.Text = file.FileName;
            }
        }
    }
}
