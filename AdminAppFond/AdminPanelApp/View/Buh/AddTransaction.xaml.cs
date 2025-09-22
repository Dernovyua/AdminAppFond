using AdminPanelApp.Logic;
using AdminPanelApp.Models;
using ClassControlsAndStyle.Dialogs;
using Microsoft.VisualBasic;
using System;
using System.Collections;
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
using System.Xml;

namespace AdminPanelApp.View
{
    /// <summary>
    /// Логика взаимодействия для AddTransaction.xaml
    /// </summary>
    public partial class AddTransaction
    {
        TransactionModel _transaction;
        AccountModel _acc;

        public AddTransaction(AccountModel acc, TransactionModel transaction)
        {
            InitializeComponent();

            _acc = acc;
            // Заполняем комбобокс счетами
            //CmbxClient.ItemsSource = LogicData.Clients.ToList();
            //if (transaction.ClientLink != null)
            //    CmbxClient.SelectedItem = transaction.ClientLink;

            // Заполняем комбобокс типами транзакций
            //CmbxType.ItemsSource = new List<TransactionType> { TransactionType.Deposit, TransactionType.Withdrawal, TransactionType.ManagementFee };
            CmbxType.DisplayMember = "Value";
            var dic = GetTransactionTypeDescriptions();
            CmbxType.ItemsSource = dic;


            _transaction = transaction;

            if (transaction.ProcessedAt.Year > 1) // Если транзакция уже существует
            {
                // Заполняем поля существующими значениями
                DtpProcessedAt.SelectedDate = transaction.ProcessedAt;
                TxbxDeposit.Text = transaction.Amount.ToString("N2");
                TxbxPaid.Text = transaction.Paid.ToString("N2");
                TxbxNotes.Text = transaction.Comment;
                if (dic.ContainsKey(transaction.Type))
                {
                    CmbxType.SelectedItem = new KeyValuePair<TransactionType, string>(
                        transaction.Type,
                        dic[transaction.Type]
                    );
                }

                //if (transaction.ClientLink != null)
                //    CmbxAccount.SelectedItem = transaction.ClientLink.Accounts.FirstOrDefault(a => a.AccountName == transaction.AccountName);

            }
            else
            {
                // Устанавливаем значения по умолчанию для новой транзакции
                DtpProcessedAt.SelectedDate = DateTime.Now;
                CmbxType.SelectedIndex = 0;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_transaction == null)
                    return;

                // Валидация выбранного счета
                //if (!(CmbxAccount.SelectedItem is AccountModel selectedAccount))
                //{
                //    new DialogMessage("Выберите счет", "Ошибка");
                //    return;
                //}

                // Валидация суммы
                if (!decimal.TryParse(TxbxDeposit.Text, out decimal amount) || amount <= 0)
                {
                    new DialogMessage("Введите корректную сумму", "Ошибка");
                    return;
                }

                //_transaction.ClientLink = (Client)CmbxClient.SelectedItem;
                // Устанавливаем значения
                _transaction.AccountId = _acc.Id;
                //_transaction.AccountName = selectedAccount.AccountName;
                // Более простой способ
                if (CmbxType.SelectedItem != null)
                {
                    var selectedPair = (KeyValuePair<TransactionType, string>)CmbxType.SelectedItem;
                    _transaction.Type = selectedPair.Key;
                }
                else
                {
                    new DialogMessage("Выберите тип транзакции", "Ошибка");
                    return;
                }

                decimal.TryParse(TxbxPaid.Text, out decimal paid);

                _transaction.Amount = amount;
                _transaction.Paid = paid;
                _transaction.ProcessedAt = (DateTime)DtpProcessedAt.SelectedDate;
                _transaction.Comment = string.IsNullOrWhiteSpace(TxbxNotes.Text) ? null : TxbxNotes.Text.Trim();
                _transaction.Status = "completed"; // или другое значение по умолчанию

                // Для новых транзакций устанавливаем дату создания
                if (_transaction.Id == 0)
                {
                    _transaction.CreatedAt = DateTime.UtcNow;
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                TxblException.Visibility = Visibility.Visible;
                TxblException.Text = $"Ошибка сохранения: {ex.Message}";
                return;
            }
            finally
            {

            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public Dictionary<TransactionType, string> GetTransactionTypeDescriptions()
        {
            return new Dictionary<TransactionType, string>
                {
                    { TransactionType.Deposit, "Пополнение" },
                    { TransactionType.Withdrawal, "Снятие" },
                    { TransactionType.ManagementFee, "Комиссия за управление" },
                    { TransactionType.SeccessFee, "Плата за успех" }
                };
        }

        private void CmbxType_SelectedChanged(object sender, RoutedEventArgs e)
        {
            var select = (KeyValuePair<TransactionType, string>)CmbxType.SelectedItem;
            TxbxPaid.IsEnabled = select.Key==TransactionType.ManagementFee || select.Key==TransactionType.SeccessFee; 
        }
    }


}
