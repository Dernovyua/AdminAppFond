using AdminPanelApp.Models;
using AdminPanelApp.Requests;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanelApp.Logic
{
    public class AdminMain
    {
        public void Execute(string path)
        {
            LogicDb.Main(path);

            LogicData.Clients = ClientsRequests.GetCliensOnLoad();

            // Запускаем периодическую проверку
            Task.Run(() => StartCheckingAsync(UpdateBalance, 1))
                .ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        Console.WriteLine($"Ошибка в StartCheckingAsync: {task.Exception.Message}");
                    }
                });
        }


       
        public async Task UpdateBalance()
        {
            var accountsWithoutStats = GetAccountsWithoutTodayStatsAsync();

            try
            {
                for (int i = LogicData.Clients.Count - 1; i >= 0; i--)
                {
                    var client = LogicData.Clients[i];

                    for (int j = client.Accounts.Count - 1; j >= 0; j--)
                    {
                        var balance = Math.Round(LogicData.Raise_OnGetBalance(client.Accounts[j].AccountNumber), 2);
                        StatisticRequests.AddStatistic(new Models.StatisticModel
                        {
                            Deposit = (decimal)balance,
                            AccountId = client.Accounts[j].Id,
                            CreatedAt = DateTime.UtcNow
                        });
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
            var today = DateTime.UtcNow.Date;

            using var connection = LogicDb.GetOpenConnection();

            // Запрос для получения счетов без статистики за сегодня
            string query = @"
                            SELECT a.* 
                            FROM accounts a
                            LEFT JOIN statistic s ON a.id = s.account_id AND s.date >= @today
                            WHERE s.account_id IS NULL;
    ";

            using var cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddWithValue("@today", today);

            var accountsWithoutStats = new List<AccountModel>();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                accountsWithoutStats.Add(new AccountModel
                {
                    Id = reader.GetInt32(0),
                    AccountName = reader.GetString(1),
                });
            }

            return accountsWithoutStats;
        }
    }
}
