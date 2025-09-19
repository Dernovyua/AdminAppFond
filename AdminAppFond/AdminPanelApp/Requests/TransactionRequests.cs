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
                   INSERT INTO transactions (account_id, type, amount, status, processed_at, comment, created_at)
                   VALUES (@accountId, @type, @amount, @status, @processedAt, @comment, @createdAt);
                   ";

            using var cmd = new SQLiteCommand(query, connection);

            cmd.Parameters.AddWithValue("@accountId", newTransaction.AccountId);
            cmd.Parameters.AddWithValue("@type", newTransaction.Type.ToString());
            cmd.Parameters.AddWithValue("@amount", newTransaction.Amount);
            cmd.Parameters.AddWithValue("@status", newTransaction.Status ?? "completed");
            cmd.Parameters.AddWithValue("@processedAt", newTransaction.ProcessedAt);
            cmd.Parameters.AddWithValue("@comment", newTransaction.Comment);
            cmd.Parameters.AddWithValue("@createdAt", newTransaction.CreatedAt);

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
                   comment=@comment
                   WHERE id = @id;";

            using var cmd = new SQLiteCommand(query, connection);

            cmd.Parameters.AddWithValue("@accountId", transaction.AccountId);
            cmd.Parameters.AddWithValue("@type", transaction.Type);
            cmd.Parameters.AddWithValue("@amount", transaction.Amount);
            cmd.Parameters.AddWithValue("@status", transaction.Status ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@processedAt", transaction.ProcessedAt);
            cmd.Parameters.AddWithValue("@id", transaction.Id);
            cmd.Parameters.AddWithValue("@comment", transaction.Comment);

            cmd.ExecuteNonQuery();
        }

        public static async Task<List<TransactionModel>> GetLastTransactions()
        {
            using var connection = LogicDb.GetOpenConnection();

            string query = @"
                            SELECT 
                                t.id,
                                t.account_id,
                                a.account_name,
                                a.account_number,
                                t.type,
                                t.amount,
                                t.status,
                                t.processed_at,
                                t.comment,
                                t.created_at
                            FROM 
                                transactions t
                            LEFT JOIN 
                                accounts a ON t.account_id = a.id
                            ORDER BY 
                                t.created_at DESC
                            LIMIT 200;
                        ";

            var transactions = new List<TransactionModel>();

            using var cmd = new SQLiteCommand(query, connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                transactions.Add(new TransactionModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    AccountId = reader.GetInt32(reader.GetOrdinal("account_id")),
                    //AccountName = reader.IsDBNull(reader.GetOrdinal("account_name"))
                    //    ? null
                    //    : reader.GetString(reader.GetOrdinal("account_name")),

                    Type = (TransactionType)Enum.Parse(typeof(TransactionType), reader.GetString(reader.GetOrdinal("type")), true),
                    Amount = reader.GetDecimal(reader.GetOrdinal("amount")),
                    Status = reader.IsDBNull(reader.GetOrdinal("status"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("status")),
                    ProcessedAt = reader.IsDBNull(reader.GetOrdinal("processed_at"))
                        ? new DateTime()
                        : reader.GetDateTime(reader.GetOrdinal("processed_at")),
                    Comment = reader.IsDBNull(reader.GetOrdinal("comment"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("comment")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                });
            }

            return transactions;
        }


    }
}
