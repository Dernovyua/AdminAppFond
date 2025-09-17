using AdminPanelApp.Logic;
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
            _client = client;
            CmbxCurrency.ItemsSource = new List<CurrencyType> { CurrencyType.USDT, CurrencyType.USDC, CurrencyType.BTC, CurrencyType.ETH, CurrencyType.RUR, CurrencyType.USD };

            LoadData();
        }

        private void LoadData()
        {
            if (_account == null)
                return;

            TxbxExchange.Text = _account.Exchange ?? string.Empty;
            TxbxAccountName.Text = _account.AccountName ?? string.Empty;

            var list = LogicData.Raise_OnGetAllAccount();
            try
            {
                //В случае удаления коннектора
                if (!String.IsNullOrEmpty(_account.AccountNumber) && list.Count(a => a.Account == _account.AccountNumber) == 0)
                    list.Add(new AllAccounts
                    {
                        Account = _account.AccountNumber,
                        Name = _account.AccountName,
                        Exchange = _account.Exchange,
                        AccString = String.IsNullOrEmpty(_account.AccountName) ? _account.AccountNumber : _account.AccountName + " (" + _account.AccountNumber + ")"
                    });
            }
            catch (Exception ex)
            {

            }
            CbmxAccount.ItemsSource = list;
            if (!String.IsNullOrEmpty(_account.AccountNumber))
                CbmxAccount.SelectedItem = list.FirstOrDefault(a => a.Account == _account.AccountNumber);
            else
                CbmxAccount.SelectedIndex = 0;

            CmbxCurrency.SelectedItem = _account.Currency;

            if (String.IsNullOrEmpty(_account.AccountNumber))
            {
                LoadAcc();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CbmxAccount.SelectedItem == null)
            {
                new DialogMessage("Необходимо настроить подключения в меню Соединения/Терминалы", "Ошибка!");
                return;
            }

            if (string.IsNullOrEmpty(_account.AccountNumber))
                _account.CreatedAt = DateTime.UtcNow;  // если есть поле UpdatedAt

            _account.Exchange = TxbxExchange.Text.Trim();
            _account.AccountName = string.IsNullOrWhiteSpace(TxbxAccountName.Text) ? null : TxbxAccountName.Text.Trim();
            _account.AccountNumber = (CbmxAccount.SelectedItem as AllAccounts).Account;
            _account.Currency = (CurrencyType)CmbxCurrency.SelectedItem;
            _account.ClientId = _client.Id;

            DialogResult = true;

            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CbmxAccount_SelectedChanged(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
                return;

            LoadAcc();
        }

        private void LoadAcc()
        {
            if (CbmxAccount.SelectedItem is AllAccounts acc)
            {
                TxbxExchange.Text = acc.Exchange ?? string.Empty;
                TxbxAccountName.Text = acc.Name ?? string.Empty;
            }
        }
    }
}
