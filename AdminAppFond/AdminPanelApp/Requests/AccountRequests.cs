using AdminPanelApp.Logic;
using AdminPanelApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanelApp.Requests
{
    public static class AccountRequests
    {
        /// <summary>
        /// Обновление счета
        /// </summary>
        /// <param name="updatedAccount"></param>
        public static void UpdateAccount(AccountModel updatedAccount)
        {
            using var conn = LogicDb.GetOpenConnection();

            using var cmd = new SQLiteCommand(conn);
            cmd.CommandText = @"
                UPDATE accounts SET
                    client_id = @clientId,
                    exchange = @exchange,
                    account_name = @accountName,
                    account_number = @accountNumber,
                    currency = @currency
                    WHERE id = @id;
                    ";

            cmd.Parameters.AddWithValue("@clientId", updatedAccount.ClientId);
            cmd.Parameters.AddWithValue("@exchange", updatedAccount.Exchange);
            cmd.Parameters.AddWithValue("@accountName", updatedAccount.AccountName);
            cmd.Parameters.AddWithValue("@accountNumber", updatedAccount.AccountNumber);
            cmd.Parameters.AddWithValue("@currency", updatedAccount.Currency);
            cmd.Parameters.AddWithValue("@id", updatedAccount.Id);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Добавление нового счета в базу
        /// </summary>
        /// <param name="newAccount"></param>
        public static void AddAccount(AccountModel newAccount)
        {
            using var conn = LogicDb.GetOpenConnection();

            using var cmd = new SQLiteCommand(conn);
            cmd.CommandText = @"
                            INSERT INTO accounts (client_id, exchange, account_name, account_number, currency)
                            VALUES (@clientId, @exchange, @accountName, @accountNumber, @currency);
                        ";

            cmd.Parameters.AddWithValue("@clientId", newAccount.ClientId);
            cmd.Parameters.AddWithValue("@exchange", newAccount.Exchange);
            cmd.Parameters.AddWithValue("@accountName", newAccount.AccountName);
            cmd.Parameters.AddWithValue("@accountNumber", newAccount.AccountNumber);
            cmd.Parameters.AddWithValue("@currency", newAccount.Currency);

            cmd.ExecuteNonQuery();

            // Получаем Id последней вставленной записи
            cmd.CommandText = "SELECT last_insert_rowid();";
            long lastId = (long)cmd.ExecuteScalar();

            newAccount.Id = (int)lastId; // присваиваем Id клиенту
        }

        /// <summary>
        /// Удаление счета из базы по Id
        /// </summary>
        /// <param name="accountId"></param>
        public static void DeleteAccount(int accountId)
        {
            using var conn = LogicDb.GetOpenConnection();

            using var cmd = new SQLiteCommand(conn);
            cmd.CommandText = "DELETE FROM accounts WHERE id = @id;";
            cmd.Parameters.AddWithValue("@id", accountId);

            cmd.ExecuteNonQuery();
        }
    }
}
