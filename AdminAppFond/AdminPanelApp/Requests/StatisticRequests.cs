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
    }
}
