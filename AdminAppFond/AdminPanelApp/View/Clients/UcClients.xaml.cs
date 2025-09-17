using AdminPanelApp.Logic;
using AdminPanelApp.Models;
using AdminPanelApp.Requests;
using ClassControlsAndStyle.Dialogs;
using ETS.Resources;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdminPanelApp.View
{
    /// <summary>
    /// Логика взаимодействия для UcClients.xaml
    /// </summary>
    public partial class UcClients
    {
        public UcClients()
        {
            InitializeComponent();

            DtgdClients.ItemsSource = LogicData.Clients;
        }

        private void DtgdClients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditClient();
        }

        private void DtgdClients_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BtnAddClient_Click(object sender, RoutedEventArgs e)
        {
            AddClient();
        }

        private void BtnCopyClient_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDelClient_Click(object sender, RoutedEventArgs e)
        {
            DelClient();
        }

        private void MnitAddClient_Click(object sender, RoutedEventArgs e)
        {
            AddClient();
        }

        private void MnitEditClient_Click(object sender, RoutedEventArgs e)
        {
            EditClient();
        }

        /// <summary>
        /// Редактировать клиента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnitDelClient_Click(object sender, RoutedEventArgs e)
        {
            DelClient();
        }


        private void AddClient()
        {
            Client client = new Client();
            AddClient add = new AddClient(client);
            add.ShowDialog();

            if (add.DialogResult == true)
            {
                LogicData.Clients.Add(client);
                ClientsRequests.AddClient(client);
            }
        }
        private void EditClient()
        {
            if (DtgdClients.SelectedItem is Client client)
            {
                AddClient add = new AddClient(client);
                add.ShowDialog();
                if (add.DialogResult == true)
                {
                    ClientsRequests.UpdateClient(client);
                }
            }
        }
        private void DelClient()
        {
            if (DtgdClients.SelectedItem is Client client)
            {
                if (new DialogOkCancel("Вы действительно хотите удалить клиента?",
                    LanguageModel.GetString(LanguageDialogMessageKeys.CaptionAttentionKey))
                    .Result == MessageBoxResult.OK)
                {
                    ClientsRequests.DeleteClient(client.Id);
                    LogicData.Clients.Remove(client);
                    LogicData.GetStatisticsAsync();
                }
            }
        }


        private void DtgdAccounts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditAccount();
        }

        private void ButtonAddAccount_Click(object sender, RoutedEventArgs e)
        {
            AddAccount();
        }

        private void BtnDeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            DelAccount();
        }

        private void MnitAddAccount_Click(object sender, RoutedEventArgs e)
        {
            AddAccount();
        }

        private void MnitEditAccount_Click(object sender, RoutedEventArgs e)
        {
            EditAccount();
        }

        private void MnitDelAccount_Click(object sender, RoutedEventArgs e)
        {
            DelAccount();
        }

        private void AddAccount()
        {
            if (DtgdClients.SelectedItem is Client client)
            {
                AccountModel account = new AccountModel();
                AccountSetting add = new AccountSetting(account, client);
                add.ShowDialog();

                if (add.DialogResult == true)
                {
                    try
                    {
                        AccountRequests.AddAccount(account);
                    }
                    catch (Exception ex)
                    {
                        new DialogMessage("Ошибка при добавлении счета" + ex.Message, "Внимание!");
                        return;
                    }
                    client.Accounts.Add(account);
                    LogicData.GetStatisticsAsync();
                }
            }
            else
            {
                new DialogMessage("Необходимо выбарть клиента", "Внимание!");
            }
        }
        private void EditAccount()
        {
            if (DtgdClients.SelectedItem is Client client)
                if (DtgdAccounts.SelectedItem is AccountModel acc)
                {
                    AccountSetting add = new AccountSetting(acc, client);
                    add.ShowDialog();
                    if (add.DialogResult == true)
                    {
                        try
                        {
                            AccountRequests.UpdateAccount(acc);
                        }
                        catch (Exception ex)
                        {
                            new DialogMessage("Ошибка при редактирования счета" + ex.Message, "Внимание!");
                            return;
                        }
                    }
                }
        }
        private void DelAccount()
        {
            if (DtgdClients.SelectedItem is Client client)
                if (DtgdAccounts.SelectedItem is AccountModel acc)
                {
                    if (new DialogOkCancel("Вы действительно хотите удалить счет?",
                        LanguageModel.GetString(LanguageDialogMessageKeys.CaptionAttentionKey))
                        .Result == MessageBoxResult.OK)
                    {
                        AccountRequests.DeleteAccount(acc.Id);
                        client.Accounts.Remove(acc);
                        LogicData.GetStatisticsAsync();
                    }
                }
        }
    }
}
