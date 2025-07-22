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
    public static class TransactionRequests
    {

        /// <summary>
        /// Добавление новой транзакции в базу
        /// </summary>
        /// <param name="newTransaction"></param>
        public static void AddTransaction(TransactionModel newTransaction)
        {
            if (newTransaction == null)
                return;

            using var connection = LogicDb.GetOpenConnection();

            string query = @"
                   INSERT INTO transactions (account_id, type, amount, status, processed_at, created_at)
                   VALUES (@accountId, @type, @amount, @status, @processedAt, @createdAt);
                   ";

            using var cmd = new SQLiteCommand(query, connection);

            cmd.Parameters.AddWithValue("@accountId", newTransaction.AccountId);
            cmd.Parameters.AddWithValue("@type", newTransaction.Type);
            cmd.Parameters.AddWithValue("@amount", newTransaction.Amount);
            cmd.Parameters.AddWithValue("@status", newTransaction.Status ?? "completed");
            cmd.Parameters.AddWithValue("@processedAt", newTransaction.ProcessedAt ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@createdAt", DateTime.Now);

            cmd.ExecuteNonQuery();

            // Получаем Id последней вставленной записи
            cmd.CommandText = "SELECT last_insert_rowid();";
            long lastId = (long)cmd.ExecuteScalar();

            newTransaction.Id = (int)lastId; // присваиваем Id транзакции
        }

        // <summary>
        /// Удаление транзакции из базы по Id
        /// </summary>
        /// <param name="transactionId"></param>
        public static void DeleteTransaction(int transactionId)
        {
            if (transactionId == 0)
                return;

            using var connection = LogicDb.GetOpenConnection();

            string query = "DELETE FROM transactions WHERE id = @id;";

            using var cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", transactionId);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Обновление данных транзакции
        /// </summary>
        /// <param name="transaction"></param>
        public static void UpdateTransaction(TransactionModel transaction)
        {
            if (transaction == null || transaction.Id == 0)
                return;

            using var connection = LogicDb.GetOpenConnection();

            string query = @"
                   UPDATE transactions SET
                   account_id = @accountId,
                   type = @type,
                   amount = @amount,
                   status = @status,
                   processed_at = @processedAt,
                   created_at = @createdAt
                   WHERE id = @id;";

            using var cmd = new SQLiteCommand(query, connection);

            cmd.Parameters.AddWithValue("@accountId", transaction.AccountId);
            cmd.Parameters.AddWithValue("@type", transaction.Type);
            cmd.Parameters.AddWithValue("@amount", transaction.Amount);
            cmd.Parameters.AddWithValue("@status", transaction.Status ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@processedAt", transaction.ProcessedAt ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@createdAt", transaction.CreatedAt);
            cmd.Parameters.AddWithValue("@id", transaction.Id);

            cmd.ExecuteNonQuery();
        }
    }
}
