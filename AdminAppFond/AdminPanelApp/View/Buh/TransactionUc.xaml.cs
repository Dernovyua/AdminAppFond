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

namespace AdminPanelApp.View.Buh
{
    /// <summary>
    /// Логика взаимодействия для TransactionUc.xaml
    /// </summary>
    public partial class TransactionUc : UserControl
    {
        public TransactionUc()
        {
            InitializeComponent();
        }
        AccountModel _acc;

        public void Load(AccountModel acc)
        {
            DtgdTransaction.ItemsSource = acc.Transaction;
            _acc = acc;
        }

        private void DtgdTransaction_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditTransaction();
        }

        private void BtnAddTransaction_Click(object sender, RoutedEventArgs e)
        {
            AddTransaction();
        }

        private void BtnDelTransaction_Click(object sender, RoutedEventArgs e)
        {
            DeleteTransaction();
        }

        private void MnitAddTransaction_Click(object sender, RoutedEventArgs e)
        {
            AddTransaction();
        }

        private void MnitEditTransaction_Click(object sender, RoutedEventArgs e)
        {
            EditTransaction();
        }

        private void MnitDelTransaction_Click(object sender, RoutedEventArgs e)
        {
            DeleteTransaction();
        }

        private void AddTransaction()
        {
            if (_acc == null)
                return;

            try
            {
                TransactionModel transaction = new TransactionModel
                {
                    ProcessedAt = DateTime.Now,
                    CreatedAt = DateTime.Now
                };

                AddTransaction add = new AddTransaction(transaction);
                add.ShowDialog();

                if (add.DialogResult == true)
                {
                    TransactionRequests.AddTransaction(transaction);
                    LogicData.Transactions.Add(transaction);
                }
            }
            catch (Exception ex)
            {
                new DialogMessage(ex.Message, "Ошибка");
            }
        }

        private void EditTransaction()
        {
            if (DtgdTransaction.SelectedItem is TransactionModel transaction)
            {
                try
                {
                    AddTransaction edit = new AddTransaction(transaction);
                    edit.ShowDialog();

                    if (edit.DialogResult == true)
                    {
                        TransactionRequests.UpdateTransaction(transaction);
                    }
                }
                catch (Exception ex)
                {

                    new DialogMessage(ex.Message, "Ошибка");
                }
            }
        }

        private void DeleteTransaction()
        {
            if (DtgdTransaction.SelectedItem is TransactionModel transaction)
            {
                try
                {
                    if (new DialogOkCancel("Вы действительно хотите удалить запись из бухгателтерии?",
                    LanguageModel.GetString(LanguageDialogMessageKeys.CaptionAttentionKey))
                    .Result == MessageBoxResult.OK)
                    {
                        TransactionRequests.DeleteTransaction(transaction.Id);
                        LogicData.Transactions.Remove(transaction);
                    }
                }
                catch (Exception ex)
                {
                    new DialogMessage(ex.Message, "Ошибка");
                }
            }
        }
    }
}
