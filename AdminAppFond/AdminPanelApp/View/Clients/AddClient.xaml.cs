using AdminPanelApp.Models;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для AddClient.xaml
    /// </summary>
    public partial class AddClient
    {

        Client _client;
        public AddClient(Client client)
        {
            InitializeComponent();
            _client = client;
            LoadClientData();
        }

        private void LoadClientData()
        {
            if (_client == null)
                return;

            TxbxFullName.Text = _client.FullName ?? string.Empty;

            if (!string.IsNullOrEmpty(_client.Status))
                CmbxStatus.SelectedItem = _client.Status;
            else
                CmbxStatus.SelectedIndex = 0;

            TxbxPhone.Text = _client.Phone ?? string.Empty;
            TxbxEmail.Text = _client.Email ?? string.Empty;
            TxbxCity.Text = _client.City ?? string.Empty;
            TxbxTelegramm.Text = _client.Telegram ?? string.Empty;
            TxbxNotes.Text = _client.Notes ?? string.Empty;
            TxbxChatId.Text = _client.ChatId.ToString();
            TxbxSucssesFee.Text = _client.SuccessFee.ToString();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(TxbxFullName.Text))
                return;

            _client.FullName = TxbxFullName.Text.Trim();
            _client.Status = CmbxStatus.SelectedItem?.ToString() ?? "active"; // например, default "active"
            _client.Phone = string.IsNullOrWhiteSpace(TxbxPhone.Text) ? null : TxbxPhone.Text.Trim();
            _client.Email = string.IsNullOrWhiteSpace(TxbxEmail.Text) ? null : TxbxEmail.Text.Trim();
            _client.City = string.IsNullOrWhiteSpace(TxbxCity.Text) ? null : TxbxCity.Text.Trim();
            _client.Telegram = string.IsNullOrWhiteSpace(TxbxTelegramm.Text) ? null : TxbxTelegramm.Text.Trim();
            _client.Notes = string.IsNullOrWhiteSpace(TxbxNotes.Text) ? null : TxbxNotes.Text.Trim();
            _client.ChatId = string.IsNullOrWhiteSpace(TxbxChatId.Text) ? 0 : Convert.ToInt64(TxbxChatId.Text.Trim());
            _client.SuccessFee = string.IsNullOrWhiteSpace(TxbxSucssesFee.Text) ? 0 : Math.Round(Convert.ToDouble(TxbxSucssesFee.Text.Trim()),2);
            _client.UpdatedAt = DateTime.UtcNow;

            if (_client.CreatedAt.Year == 1)
                _client.CreatedAt = DateTime.UtcNow;

            DialogResult = true;

            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
