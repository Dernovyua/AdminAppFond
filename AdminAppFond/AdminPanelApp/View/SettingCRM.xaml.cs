using AdminPanelApp.Logic;
using ClassControlsAndStyle.Dialogs;
using DevExpress.XtraSpellChecker;
using ETS.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using static AdminPanelApp.View.Scenario.AddScenario;

namespace AdminPanelApp.View
{
    /// <summary>
    /// Логика взаимодействия для SettingCRM.xaml
    /// </summary>
    public partial class SettingCRM
    {
        public SettingCRM()
        {
            InitializeComponent();

            TxbxTokenAdmin.Text = LogicData.SettingCrm.TgTokenCrm;
            TxbxHandMessage.PreviewMouseRightButtonUp += RichTextBox_PreviewMouseRightButtonUp;
            if (String.IsNullOrEmpty(LogicData.SettingCrm.MessageHandBuh))
                LogicData.SettingCrm.MessageHandBuh = "Поздравляем, вы получили доход!\n\n<b>Статистика</b>\nПрибыль по счёту за текущий период: {Profit} (в USDT)\n" +
                    "Ваша актуальная комиссия: {SuccessFee}%\nДобровольное отчисление составляет: {Comis}\n\nБлагодарим за взаимовыгодное сотрудничество!";
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            LogicData.SettingCrm.TgTokenCrm = TxbxTokenAdmin.Text;
            LogicData.SettingCrm.MessageHandBuh = SimpleTelegramFormatter.ToTelegramHtml(TxbxHandMessage);

            DialogResult = true;

            Close();
        }

        private void BtnGide_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://etstrading.ru/bz_nastroyki_uvedomleniya") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                new DialogMessage(LanguageModel.GetString(LanguageCommonKeys.ErrorOpenKey) + " " + ex.Message, LanguageModel.GetString(LanguageDialogMessageKeys.CaptionAttentionKey));
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            TxbxHandMessage.Copy();
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            TxbxHandMessage.Paste();
        }

        // Загрузить из строки (автоопределение формата)
        private void LoadFromString(string text)
        {
            if (String.IsNullOrEmpty(text))
                return;

            if (text.Contains("<Section") && text.Contains("xmlns="))
            {
                // XAML формат
                RichTextBoxLoader.LoadFromXaml(TxbxHandMessage, text);
            }
            else if (text.Contains("<") && text.Contains(">"))
            {
                // HTML формат
                RichTextBoxLoader.LoadFromHtml(TxbxHandMessage, text);
            }
            else
            {
                // Простой текст
                RichTextBoxLoader.LoadPlainText(TxbxHandMessage, text);
            }
        }

        private void RichTextBox_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Обновляем состояние меню перед показом
            UpdateMenuItemsState();
        }

        private void UpdateMenuItemsState()
        {
            // Проверяем, есть ли выделение
            bool hasSelection = !TxbxHandMessage.Selection.IsEmpty;

            // Находим элементы меню и обновляем их состояние
            var contextMenu = TxbxHandMessage.ContextMenu;
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
            if (TxbxHandMessage.Selection.IsEmpty)
                return;

            var currentWeight = TxbxHandMessage.Selection.GetPropertyValue(TextElement.FontWeightProperty);
            var newWeight = currentWeight.Equals(FontWeights.Bold) ?
                FontWeights.Normal : FontWeights.Bold;

            TxbxHandMessage.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, newWeight);
        }

        private void MakeItalic_Click(object sender, RoutedEventArgs e)
        {
            if (TxbxHandMessage.Selection.IsEmpty)
                return;

            var currentStyle = TxbxHandMessage.Selection.GetPropertyValue(TextElement.FontStyleProperty);
            var newStyle = currentStyle.Equals(FontStyles.Italic) ?
                FontStyles.Normal : FontStyles.Italic;

            TxbxHandMessage.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, newStyle);
        }

        private void AddHyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (TxbxHandMessage.Selection.IsEmpty)
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
                    TxbxHandMessage.Selection.Start,
                    TxbxHandMessage.Selection.End).Text;

                // Создаем гиперссылку
                var hyperlink = new Hyperlink(TxbxHandMessage.Selection.Start, TxbxHandMessage.Selection.End)
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


    }
}
