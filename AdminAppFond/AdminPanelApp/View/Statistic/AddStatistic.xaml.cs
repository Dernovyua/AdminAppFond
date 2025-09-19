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
    /// Логика взаимодействия для AddStatistic.xaml
    /// </summary>
    public partial class AddStatistic
    {
        StatisticModel _statistic;

        public AddStatistic(StatisticModel statistic, StatisticDisplayModel stat)
        {
            InitializeComponent();

            CmbxAccount.ItemsSource = LogicData.Clients.SelectMany(client => client.Accounts).ToList();

            _statistic = statistic;


            if (CmbxAccount.ItemsSource is IEnumerable items)
            {
                if (statistic.AccountId > 0)
                {
                    var account = items.OfType<AccountModel>().FirstOrDefault(a => a.Id == statistic.AccountId);
                    if (account != null)
                    {
                        CmbxAccount.SelectedItem = account;
                    }
                    else
                    {
                        // Дополнительная обработка если счет не найден
                        Debug.WriteLine($"Счет с ID {statistic.AccountId} не найден в списке");
                    }
                }
                else
                {
                    CmbxAccount.SelectedItem = stat.Account;
                }
            }
            if (statistic.CreatedAt.Year > 1)
            {
                DtpDate.SelectedDate = statistic.Date;
                TxbxNotes.Text = statistic.Comment;
                TxbxDeposit.Text = statistic.Deposit.ToString();
            }
            else
            {
                DtpDate.SelectedDate = DateTime.Now;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (_statistic == null)
                    return;

                // Если это новая запись, устанавливаем дату создания
                if (_statistic.Id == 0)
                {
                    _statistic.CreatedAt = DateTime.UtcNow;
                }

                // Парсим и устанавливаем значения
                _statistic.Date = DtpDate.SelectedDate ?? DateTime.Now;

                if (decimal.TryParse(TxbxDeposit.Text, out decimal deposit))
                {
                    _statistic.Deposit = deposit;
                }
                else
                {
                    // Обработка ошибки ввода суммы
                    new DialogMessage("Некорректная сумма депозита", "Ошибка");
                    return;
                }

                _statistic.Comment = string.IsNullOrWhiteSpace(TxbxNotes.Text) ? null : TxbxNotes.Text.Trim();
                _statistic.AccountId = (CmbxAccount.SelectedItem as AccountModel).Id; // Предполагается, что _account передается в форму
                _statistic.AccountName = (CmbxAccount.SelectedItem as AccountModel).AccountName;

                DialogResult = true;
            }
            catch (Exception ex)
            {
                new DialogMessage(ex.Message, "Ошибка");
            }
            Close();

        }


        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
