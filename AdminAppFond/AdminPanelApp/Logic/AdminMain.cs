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

            GetStatistic();
        }

        private void GetStatistic()
        { 
            var stat = StatisticRequests.GetStatistics();
            foreach (var item in stat)
            {
                LogicData.Statistics.Add(item);
            }
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
                        if (!Double.IsNaN(balance))
                            StatisticRequests.AddStatistic(new Models.StatisticModel
                            {
                                Deposit = (decimal)balance,
                                AccountId = client.Accounts[j].Id,
                                Date = DateTime.UtcNow
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

            // Улучшенный запрос с явным указанием столбцов
            string query = @"
                            SELECT a.id, a.account_name, a.account_number 
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
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    AccountName = reader.GetString(reader.GetOrdinal("account_name")),
                    AccountNumber = reader.IsDBNull(reader.GetOrdinal("account_number"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("account_number"))
                });
            }

            return accountsWithoutStats;
        }
    }
}
