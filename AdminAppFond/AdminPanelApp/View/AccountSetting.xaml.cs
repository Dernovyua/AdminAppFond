using AdminPanelApp.Models;
using ClassControlsAndStyle.Dialogs;
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
    /// Логика взаимодействия для AccountSetting.xaml
    /// </summary>
    public partial class AccountSetting 
    {
        AccountModel _account;
        Client _client;
        public AccountSetting(AccountModel account, Client client)
        {
            InitializeComponent();
            _account = account;
            _client= client;
            CmbxCurrency.ItemsSource = new List<CurrencyType> { CurrencyType.USDT, CurrencyType.USDC, CurrencyType.BTC, CurrencyType.ETH, CurrencyType.RUR, CurrencyType.USD};

            LoadData();
        }

        private void LoadData()
        {
            if (_account == null)
                return;

            TxbxExchange.Text = _account.Exchange ?? string.Empty;
            TxbxAccountName.Text = _account.AccountName ?? string.Empty;
            TxbxAccountNumber.Text = _account.AccountNumber ?? string.Empty;
            CmbxCurrency.Text = _account.Currency.ToString() ?? string.Empty;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxbxAccountNumber.Text))
            {
                new DialogMessage("Необходимо указать название счета", "Ошибка!");
                return;
            }

            if (string.IsNullOrEmpty(_account.AccountNumber))
                _account.CreatedAt = DateTime.UtcNow;  // если есть поле UpdatedAt

            _account.Exchange = TxbxExchange.Text.Trim();
            _account.AccountName = string.IsNullOrWhiteSpace(TxbxAccountName.Text) ? null : TxbxAccountName.Text.Trim();
            _account.AccountNumber = string.IsNullOrWhiteSpace(TxbxAccountNumber.Text) ? null : TxbxAccountNumber.Text.Trim();
            _account.ClientId = _client.Id;

            DialogResult = true;

            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
