using AdminPanelApp.Logic;
using AdminPanelApp.Models;
using ClassControlsAndStyle.Dialogs;
using Microsoft.VisualBasic;
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
    /// Логика взаимодействия для AddStatistic.xaml
    /// </summary>
    public partial class AddStatistic
    {
        StatisticModel _statistic;

        public AddStatistic(StatisticModel statistic, int accountId = 0)
        {
            InitializeComponent();

            CmbxAccount.ItemsSource = LogicData.Clients.SelectMany(client => client.Accounts).ToList();
            _statistic = statistic;
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
