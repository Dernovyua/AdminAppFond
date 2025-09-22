using AdminPanelApp.Logic;
using AdminPanelApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanelApp.Requests
{
    public static class StatisticRequests
    {
        /// <summary>
        /// Добавление новой записи статистики в базу
        /// </summary>
        /// <param name="statistic"></param>
        public static void AddStatistic(StatisticModel statistic)
        {
            if (statistic == null)
                return;

            using var connection = LogicDb.GetOpenConnection();

            string query = @"
                   INSERT INTO statistic (date, deposit, account_id, comment)
                   VALUES (@date, @deposit, @accountId, @comment);
                   ";

            using var cmd = new SQLiteCommand(query, connection);

            cmd.Parameters.AddWithValue("@date", statistic.Date);
            cmd.Parameters.AddWithValue("@deposit", statistic.Deposit);
            cmd.Parameters.AddWithValue("@accountId", statistic.AccountId);
            cmd.Parameters.AddWithValue("@comment", statistic.Comment ?? (object)DBNull.Value);

            cmd.ExecuteNonQuery();

            // Получаем Id последней вставленной записи
            cmd.CommandText = "SELECT last_insert_rowid();";
            long lastId = (long)cmd.ExecuteScalar();

            statistic.Id = (int)lastId; // присваиваем Id записи статистики
        }

        /// <summary>
        /// Удаление записи статистики из базы по Id
        /// </summary>
        /// <param name="statisticId"></param>
        public static void DeleteStatistic(int statisticId)
        {
            if (statisticId == 0)
                return;

            using var connection = LogicDb.GetOpenConnection();

            string query = "DELETE FROM statistic WHERE id = @id;";

            using var cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", statisticId);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Обновление записи статистики
        /// </summary>
        /// <param name="statistic"></param>
        public static void UpdateStatistic(StatisticModel statistic)
        {
            if (statistic == null || statistic.Id == 0)
                return;

            using var connection = LogicDb.GetOpenConnection();

            string query = @"
                   UPDATE statistic SET
                   date = @date,
                   deposit = @deposit,
                   account_id = @accountId,
                   comment = @comment
                   WHERE id = @id;";

            using var cmd = new SQLiteCommand(query, connection);

            cmd.Parameters.AddWithValue("@date", statistic.Date);
            cmd.Parameters.AddWithValue("@deposit", statistic.Deposit);
            cmd.Parameters.AddWithValue("@accountId", statistic.AccountId);
            cmd.Parameters.AddWithValue("@comment", statistic.Comment ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@id", statistic.Id);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Получение записей статистики по идентификатору счета
        /// </summary>
        /// <param name="accountId">Идентификатор счета</param>
        /// <returns>Список объектов StatisticModel для указанного счета</returns>
        public static async Task<List<StatisticModel>> GetStatisticsByAccountId(int accountId)
        {
            var statistics = new List<StatisticModel>();

            using var connection = LogicDb.GetOpenConnection();

            string query = @"
                            SELECT id, date, deposit, account_id, comment, created_at
                            FROM statistic
                            WHERE account_id = @accountId
                            ORDER BY date DESC;
    ";

            using var cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddWithValue("@accountId", accountId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var statistic = new StatisticModel
                {
                    Id = reader.GetInt32(0),
                    Date = reader.GetDateTime(1),
                    Deposit = reader.GetDecimal(2),
                    AccountId = reader.GetInt32(3),
                    Comment = reader.IsDBNull(4) ? null : reader.GetString(4),
                    CreatedAt = reader.GetDateTime(5)
                };

                statistics.Add(statistic);
            }

            return statistics;
        }

        /// <summary>
        /// Получение списка записей статистики из базы данных
        /// </summary>
        /// <returns>Список объектов StatisticModel</returns>
        public static async Task<List<StatisticModel>> GetStatistics()
        {
            var statistics = new List<StatisticModel>();

            using var connection = LogicDb.GetOpenConnection();

            string query = @"
                SELECT 
                    s.id, 
                    s.date, 
                    s.deposit, 
                    s.account_id, 
                    a.account_name,
                    a.account_number,
                    s.comment, 
                    s.created_at
                FROM 
                    statistic s
                LEFT JOIN 
                    accounts a ON s.account_id = a.id
                ORDER BY 
                    s.date DESC
                LIMIT 100;
            ";

            using var cmd = new SQLiteCommand(query, connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                try
                {
                    var statistic = new StatisticModel
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Date = reader.GetDateTime(reader.GetOrdinal("date")),
                        Deposit = reader.GetDecimal(reader.GetOrdinal("deposit")),
                        AccountId = reader.GetInt32(reader.GetOrdinal("account_id")),
                        //AccountName = reader.GetString(reader.GetOrdinal("account_name")),
                        Comment = reader.IsDBNull(reader.GetOrdinal("comment"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("comment")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                    };

                    statistics.Add(statistic);
                }
                catch (Exception ex)
                {

                }

            }

            return statistics;
        }
    }
}
