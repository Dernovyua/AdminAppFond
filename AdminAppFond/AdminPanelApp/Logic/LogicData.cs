using AdminPanelApp.Models;
using AdminPanelApp.Requests;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdminPanelApp.Logic
{
    public static class LogicData
    {
        public static event Func<string, string, double> OnGetBalance;

        public static double Raise_OnGetBalance(string publicKey, string currency)
        {
            if (OnGetBalance != null)
            {
                // Вызов всех подписчиков, взять результат первого
                foreach (Func<string, string, double> handler in OnGetBalance.GetInvocationList())
                {
                    return handler(publicKey, currency);
                }
            }
            return 0;
        }

        public static ObservableCollection<Client> Clients = new ObservableCollection<Client>();
        public static ObservableCollection<StatisticModel> Statistics = new ObservableCollection<StatisticModel>();
        public static ObservableCollection<TransactionModel> Transactions = new ObservableCollection<TransactionModel>();

        public static async Task GetTransactionsAsync()
        {
            var transactions = await TransactionRequests.GetLastTransactions();
            Application.Current.Dispatcher.Invoke(() =>
            {
                LogicData.Transactions = new ObservableCollection<TransactionModel>(transactions);
            });
        }

        public static async Task GetStatisticsAsync()
        {
            var stats = await StatisticRequests.GetStatistics();
            Application.Current.Dispatcher.Invoke(() =>
            {
                LogicData.Statistics = new ObservableCollection<StatisticModel>(stats);
            });
        }

    }
}
