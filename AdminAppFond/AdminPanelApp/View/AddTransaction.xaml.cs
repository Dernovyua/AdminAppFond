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

namespace AdminPanelApp.View
{
    /// <summary>
    /// Логика взаимодействия для AddTransaction.xaml
    /// </summary>
    public partial class AddTransaction 
    {
        TransactionModel _transaction;

        public AddTransaction(TransactionModel transaction)
        {
            InitializeComponent();

            // Заполняем комбобокс счетами
            CmbxAccount.ItemsSource = LogicData.Clients.SelectMany(client => client.Accounts).ToList();

            // Заполняем комбобокс типами транзакций
            CmbxType.ItemsSource = new List<TransactionType> { TransactionType.Deposit, TransactionType.Withdrawal, TransactionType.ManagementFee };

            _transaction = transaction;

            if (transaction.CreatedAt.Year > 1) // Если транзакция уже существует
            {
                // Заполняем поля существующими значениями
                DtpProcessedAt.SelectedDate = transaction.ProcessedAt ?? DateTime.Now;
                TxbxDeposit.Text = transaction.Amount.ToString("N2");
                TxbxNotes.Text = transaction.Comment;
                CmbxType.SelectedItem = transaction.Type;

                // Выбираем соответствующий счет
                if (CmbxAccount.ItemsSource is IEnumerable items && transaction.AccountId > 0)
                {
                    var account = items.OfType<AccountModel>().FirstOrDefault(a => a.Id == transaction.AccountId);
                    if (account != null)
                    {
                        CmbxAccount.SelectedItem = account;
                    }
                    else
                    {
                        // Дополнительная обработка если счет не найден
                        Debug.WriteLine($"Счет с ID {transaction.AccountId} не найден в списке");
                    }
                }
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
                if (!(CmbxAccount.SelectedItem is AccountModel selectedAccount))
                {
                    new DialogMessage("Выберите счет", "Ошибка");
                    return;
                }

                // Валидация суммы
                if (!decimal.TryParse(TxbxDeposit.Text, out decimal amount) || amount <= 0)
                {
                    new DialogMessage("Введите корректную сумму", "Ошибка");
                    return;
                }

                // Устанавливаем значения
                _transaction.AccountId = selectedAccount.Id;
                _transaction.AccountName = selectedAccount.AccountName;
                if (CmbxType.SelectedItem is TransactionType selectedType)
                {
                    _transaction.Type = selectedType;
                }
                else
                {
                    new DialogMessage("Выберите тип транзакции", "Ошибка");
                    return;
                }

                _transaction.Amount = amount;
                _transaction.ProcessedAt = DtpProcessedAt.SelectedDate;
                _transaction.Comment = string.IsNullOrWhiteSpace(TxbxNotes.Text) ? null : TxbxNotes.Text.Trim();
                _transaction.Status = "completed"; // или другое значение по умолчанию

                // Для новых транзакций устанавливаем дату создания
                if (_transaction.Id == 0)
                {
                    _transaction.CreatedAt = DateTime.UtcNow;
                }

                DialogResult = true;
            }
            catch (Exception ex)
            {
                new DialogMessage($"Ошибка сохранения: {ex.Message}", "Ошибка");
            }
            finally
            {
                Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
