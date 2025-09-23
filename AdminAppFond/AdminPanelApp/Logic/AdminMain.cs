using AdminPanelApp.Models;
using AdminPanelApp.Requests;
using AdminPanelApp.View;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Telegram.Bot.Types;

namespace AdminPanelApp.Logic
{
    public class AdminMain
    {
        string _pathSave = "";

        public void Execute(string path)
        {
            _pathSave = path;
            LogicDb.Main(path);

            LogicData.Clients = ClientsRequests.GetCliensOnLoad();
            foreach (var item in LogicData.Clients)
            {
                MessagesRequests.LoadClientMessages(item);
            }

            // Запускаем периодическую проверку
            Task.Run(() => StartCheckingAsync(UpdateBalance, 1))
                .ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        Console.WriteLine($"Ошибка в StartCheckingAsync: {task.Exception.Message}");
                    }
                });

            LogicData.GetStatisticsAsync();
            LogicData.GetTransactionsAsync();
            LogicData.GetTgMenusAsync();

            LogicData.LoadSettingCrm(_pathSave);

            _ = LogicData.TgBot.CreateTgBot();

        }

        public void OpenWindowSettingCrm()
        {
            SettingCRM win = new SettingCRM();
            win.ShowDialog();

            if (win.DialogResult == true)
            {
                LogicData.SaveSettingCrm(_pathSave);
                _ = LogicData.TgBot.CreateTgBot();
            }
        }


        public async Task UpdateBalance()
        {
            var accountsWithoutStats = await GetAccountsWithoutTodayStatsAsync();

            try
            {
                for (int i = LogicData.Clients.Count - 1; i >= 0; i--)
                {
                    var client = LogicData.Clients[i];

                    for (int j = client.Accounts.Count - 1; j >= 0; j--)
                    {
                        LogicData.RaiseOnSendMessage("Проверяем счет: " + client.Accounts[j].AccountName + "  /  " + client.Accounts[j].AccountNumber, false);
                        if (client.Accounts[j].LastDateAddBalanceToStat.Date < DateTime.UtcNow.Date)
                        {
                            LogicData.RaiseOnSendMessage("За текущую дату нет данных в статистике", false);

                            if (accountsWithoutStats.Count(a => a.AccountNumber == client.Accounts[j].AccountNumber && //При перезапуске в этот же день
                                a.CreatedAt.Date == DateTime.UtcNow.Date) == 1)
                            {
                                LogicData.RaiseOnSendMessage("Данные за текущую дату есть: " + DateTime.UtcNow.Date, false);
                                client.Accounts[j].LastDateAddBalanceToStat = DateTime.UtcNow;
                                continue;
                            }

                            var balance = Math.Round(LogicData.Raise_OnGetBalance(client.Accounts[j].AccountNumber, client.Accounts[j].Currency.ToString()), 2);
                            LogicData.RaiseOnSendMessage("Баланс=" + balance, false);

                            if (!Double.IsNaN(balance) && balance > 0.0000001)
                            {
                                LogicData.RaiseOnSendMessage("Добавляем в статистику шаг 1", false);
                                var stat = new Models.StatisticModel
                                {
                                    Deposit = (decimal)balance,
                                    AccountId = client.Accounts[j].Id,
                                    Date = DateTime.UtcNow
                                };
                                LogicData.RaiseOnSendMessage("Добавляем в статистику шаг 2", false);

                                StatisticRequests.AddStatistic(stat);
                                client.Accounts[j].LastDateAddBalanceToStat = DateTime.UtcNow;
                                LogicData.RaiseOnSendMessage("Закончили", false);
                                BuhCalc.SetPnl(client.Accounts[j]);
                                BuhCalc.UpdateResultClient(client);
                                BuhCalc.CalcSuccessFee(client.Accounts[j]);

                            }
                            //balance = Math.Round(LogicData.Raise_OnGetBalance(client.Accounts[j].AccountNumber, client.Accounts[j].Currency.ToString()), 2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                Console.WriteLine($"Ошибка в UpdateBalance: {ex.Message}");
                throw; // Пробрасываем исключение дальше
            }
        }

        private CancellationTokenSource _cts;

        public async Task StartCheckingAsync(Func<Task> functionToCheck,
                                            int intervalInMinutes = 1,
                                            CancellationToken cancellationToken = default)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var periodicTimer = new PeriodicTimer(TimeSpan.FromMinutes(intervalInMinutes));

            try
            {
                while (await periodicTimer.WaitForNextTickAsync(_cts.Token))
                {
                    await functionToCheck();
                }
            }
            catch (OperationCanceledException)
            {
                // Обработка отмены
            }
        }

        public void StopChecking()
        {
            _cts?.Cancel();
        }

        /// <summary>
        /// Получает все счета из БД, по которым нет записей статистики за текущую дату
        /// Нужно учесть что собираем данные только по подлкюченным счетам
        /// </summary>
        /// <returns>Список счетов без статистики за сегодня</returns>
        public async Task<List<AccountModel>> GetAccountsWithoutTodayStatsAsync()
        {
            using var connection = LogicDb.GetOpenConnection();

            // Запрос для получения всех счетов с последней датой статистики
            string query = @"
        SELECT 
            a.id, 
            a.account_name, 
            a.account_number,
            a.currency,
            MAX(s.date) AS last_stat_date
        FROM 
            accounts a
        LEFT JOIN 
            statistic s ON a.id = s.account_id
        GROUP BY
            a.id, a.account_name, a.account_number
        ORDER BY
            a.account_name";

            using var cmd = new SQLiteCommand(query, connection);

            var accounts = new List<AccountModel>();

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var currencyValue = reader.IsDBNull(reader.GetOrdinal("currency"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("currency"));

                accounts.Add(new AccountModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    AccountName = reader.GetString(reader.GetOrdinal("account_name")),
                    Currency = string.IsNullOrEmpty(currencyValue)
                                ? CurrencyType.USDT // Значение по умолчанию
                                : (CurrencyType)Enum.Parse(typeof(CurrencyType), currencyValue, true),
                    AccountNumber = reader.IsDBNull(reader.GetOrdinal("account_number"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("account_number")),
                    CreatedAt = reader.IsDBNull(reader.GetOrdinal("last_stat_date"))
                        ? DateTime.MinValue
                        : reader.GetDateTime(reader.GetOrdinal("last_stat_date"))
                });
            }

            return accounts;
        }
    }
}
