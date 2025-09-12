using AdminPanelApp.Logic;
using AdminPanelApp.Models;
using DevExpress.Utils;
using DevExpress.XtraCharts.Native;
using Ex.UI.Kit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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

namespace AdminPanelApp.View
{
    /// <summary>
    /// Логика взаимодействия для DialogsView.xaml
    /// </summary>
    public partial class DialogsView : ExDockDocumentPanel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public DialogsView()
        {
            InitializeComponent();

            ChatList.ItemsSource = LogicData.Clients;
            //DataContext = LogicData.Clients;

            DataContext = this;
        }

        private Client _selectedChat;
        public Client SelectedChat
        {
            get => _selectedChat;
            set
            {
                _selectedChat = value;
                OnPropertyChanged(nameof(SelectedChat));
            }
        }


        private void Chat_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as Border).DataContext is Client client)
            {
                foreach (var c in LogicData.Clients)
                    c.IsSelected = false;

                client.IsSelected = true;
                client.Chat.IsUnread = false;

                SelectedChat = client;
            }
        }

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(TxbxMessage.Text) &&
                !String.IsNullOrWhiteSpace(TxbxMessage.Text) &&
                SelectedChat != null && SelectedChat.ChatId > 0)
            {
                LogicData.TgBot.SendMeessageToUserFromAdmin(SelectedChat.ChatId, TxbxMessage.Text.Trim(), null);
            }
            TxbxMessage.Text = "";
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Проверяем сочетание Ctrl+Enter
            if (e.Key == Key.Enter && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                // Вызываем обработчик кнопки
                SendMessage_Click(sender, e);

                // Помечаем событие как обработанное
                e.Handled = true;
            }
        }
    }


    public class BoolToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; } = false;
        public bool CollapseInsteadOfHide { get; set; } = true;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = false;

            if (value is bool boolVal)
                flag = boolVal;

            if (Invert)
                flag = !flag;

            if (flag)
                return Visibility.Visible;

            return CollapseInsteadOfHide ? Visibility.Collapsed : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                bool result = visibility == Visibility.Visible;
                return Invert ? !result : result;
            }

            return DependencyProperty.UnsetValue;
        }
    }

    /// <summary>
    /// Выбирает фон для пузырька сообщения:
    /// своё — один цвет, чужое — другой.
    /// </summary>
    public class MessageBubbleBackgroundConverter : IValueConverter
    {
        /// <param name="value">bool IsOwn</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isOwn = false;
            if (value is bool b) isOwn = b;

            //// ключи кистей в ресурсах
            //var key = isOwn
            //    ? "OwnMessageBubbleBackground"
            //    : "OtherMessageBubbleBackground";

            //if (Application.Current.Resources.Contains(key))
            //    return Application.Current.Resources[key] as Brush;
            
            // fallback
            return isOwn
                //? new SolidColorBrush(Color.FromRgb(220, 248, 198))   // светло‑зелёный
                //? new SolidColorBrush(Color.FromRgb(0, 66, 100))
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(ExThemeManager.AllThemes[ExThemeManager.Instance.CurentTheme]["Ex-SimpleTable-Row-Background-IsSelected-True"].ToString()))
                :  new SolidColorBrush((Color)ColorConverter.ConvertFromString(ExThemeManager.AllThemes[ExThemeManager.Instance.CurentTheme]["Ex-SimpleTable-AlternativeRow-Background"].ToString()));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Выравнивает пузырёк сообщения по левому или правому краю
    /// в зависимости от того, своё это сообщение или чужое.
    /// </summary>
    public class MessageAlignmentConverter : IValueConverter
    {
        /// <param name="value">bool IsOwn</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isOwn = false;
            if (value is bool b) isOwn = b;
            return isOwn ? HorizontalAlignment.Left : HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // не предполагается обратного биндинга
            throw new NotImplementedException();
        }
    }
}
