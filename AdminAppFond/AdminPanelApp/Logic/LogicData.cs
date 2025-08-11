using AdminPanelApp.Models;
using AdminPanelApp.Requests;
using AdminPanelApp.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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

        /// <summary>
        /// Делегат для отправки сообщения.
        /// </summary>
        /// <param name="message">Текст сообщения.</param>
        /// <param name="showWinMessage">Показать ли сообщение победы.</param>
        public delegate void SendMessage(string message, bool showWinMessage = false);

        /// <summary>
        /// Глобальное событие отправки сообщения.
        /// </summary>
        public static event SendMessage? OnSendMessage;

        /// <summary>
        /// Вызывает событие <see cref="OnSendMessage"/> с заданными параметрами.
        /// </summary>
        /// <param name="message">Текст сообщения.</param>
        /// <param name="showWinMessage">Показать ли сообщение победы.</param>
        public static void RaiseOnSendMessage(string message, bool showWinMessage = false)
        {
            OnSendMessage?.Invoke(message, showWinMessage);
        }

        public static TelegramBot TgBot = new TelegramBot();

        public static SettingCrmModel SettingCrm = new SettingCrmModel();
        public static ObservableCollection<Client> Clients = new ObservableCollection<Client>();
        public static ObservableCollection<TransactionModel> Transactions = new ObservableCollection<TransactionModel>();
        public static ObservableCollection<StatisticDisplayModel> StatisticDisplay = new ObservableCollection<StatisticDisplayModel>();

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

            var allStats = await StatisticRequests.GetStatistics();

            // Группируем по AccountId
            var statsByAccount = allStats
                .GroupBy(s => s.AccountId)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var client in Clients)
            {
                foreach (var account in client.Accounts)
                {
                    if (statsByAccount.TryGetValue(account.Id, out var stats))
                        account.Statistics = new ObservableCollection<StatisticModel>(stats);
                    else
                        account.Statistics = new ObservableCollection<StatisticModel>();
                }
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                UpdateStatistic();
            });
        }


        private static void UpdateStatistic()
        {
            StatisticDisplay.Clear();

            for (int i = Clients.Count - 1; i >= 0; i--)
            {
                var client = Clients[i];
                for (int j = client.Accounts.Count - 1; j >= 0; j--)
                {
                    StatisticDisplay.Add(new StatisticDisplayModel
                    {
                        ClientName = client,
                        Account = client.Accounts[j]
                    });
                }
            }
        }

        static string _pathSetting = "SettingCrm.json";

        public static void SaveSettingCrm(string pathSave)
        {
            try
            {
                String path = pathSave;

                if (!Directory.Exists(path))
                {
                    DirectoryInfo dir = new DirectoryInfo(path);
                    dir.Create();
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                path = System.IO.Path.Combine(path, _pathSetting);

                string json = JsonSerializer.Serialize(SettingCrm, options);
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                RaiseOnSendMessage(ex.Message);
            }
        }

        public static void LoadSettingCrm(string pathSave)
        {
            try
            {
                String path = pathSave;

                if (!Directory.Exists(path))
                {
                    DirectoryInfo dir = new DirectoryInfo(path);
                    dir.Create();
                }
                path = System.IO.Path.Combine(pathSave, _pathSetting);
                if (!File.Exists(path))
                    return;

                string json = File.ReadAllText(path);
                var loadedData = JsonSerializer.Deserialize<SettingCrmModel>(json);

                if (loadedData != null)
                    SettingCrm = loadedData;
            }
            catch (Exception ex)
            {
                RaiseOnSendMessage(ex.Message);
            }
        }
    }
}
