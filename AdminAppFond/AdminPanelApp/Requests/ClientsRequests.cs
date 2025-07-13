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
    public static class ClientsRequests
    {
        /// <summary>
        /// Получение всех клиентов с их счетами.
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<Client> GetCliensOnLoad()
        {
            var clientsDict = new Dictionary<int, Client>();

            using var conn = LogicDb.GetOpenConnection();
            conn.Open();

            using var cmd = new SQLiteCommand(@"
                                                SELECT 
                                                    c.id AS ClientId, c.full_name, c.opened_at, c.status, c.notes, c.phone, c.email, c.telegram, c.city, c.created_at, c.updated_at,
                                                    a.id AS AccountId, a.client_id, a.exchange, a.account_name, a.account_number, a.created_at AS AccountCreatedAt
                                                FROM clients c
                                                LEFT JOIN accounts a ON c.id = a.client_id
                                                ORDER BY c.id;
                                            ", conn);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var clientId = reader.GetInt32(reader.GetOrdinal("ClientId"));

                if (!clientsDict.TryGetValue(clientId, out var client))
                {
                    client = new Client
                    {
                        Id = clientId,
                        FullName = reader.GetString(reader.GetOrdinal("full_name")),
                        Status = reader.GetString(reader.GetOrdinal("status")),
                        Notes = reader.IsDBNull(reader.GetOrdinal("notes")) ? null : reader.GetString(reader.GetOrdinal("notes")),
                        Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString(reader.GetOrdinal("phone")),
                        Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                        Telegram = reader.IsDBNull(reader.GetOrdinal("telegram")) ? null : reader.GetString(reader.GetOrdinal("telegram")),
                        City = reader.IsDBNull(reader.GetOrdinal("city")) ? null : reader.GetString(reader.GetOrdinal("city")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                        UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
                        Accounts = new System.Collections.ObjectModel.ObservableCollection<AccountModel>()
                    };
                    clientsDict.Add(clientId, client);
                }

                // Если аккаунт есть (LEFT JOIN может вернуть NULL)
                if (!reader.IsDBNull(reader.GetOrdinal("AccountId")))
                {
                    var account = new AccountModel
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("AccountId")),
                        ClientId = clientId,
                        Exchange = reader.GetString(reader.GetOrdinal("exchange")),
                        AccountName = reader.GetString(reader.GetOrdinal("account_name")),
                        AccountNumber = reader.GetString(reader.GetOrdinal("account_number")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("AccountCreatedAt"))
                    };
                    client.Accounts.Add(account);
                }
            }

            return new ObservableCollection<Client>(clientsDict.Values);
        }

        /// <summary>
        /// Добавление нового клиента в базу
        /// </summary>
        /// <param name="newClient"></param>
        public static void AddClient(Client newClient)
        {
            if (newClient == null)
                return;

            using var connection = LogicDb.GetOpenConnection();

            string query = @"
                           INSERT INTO clients (full_name, opened_at, status, notes, phone, email, telegram, city, created_at, updated_at)
                           VALUES (@fullName, @openedAt, @status, @notes, @phone, @email, @telegram, @city, @createdAt, @updatedAt);
                           ";

            using var cmd = new SQLiteCommand(query, connection);

            cmd.Parameters.AddWithValue("@fullName", newClient.FullName);
            cmd.Parameters.AddWithValue("@status", newClient.Status);
            cmd.Parameters.AddWithValue("@notes", newClient.Notes ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@phone", newClient.Phone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@email", newClient.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@telegram", newClient.Telegram ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@city", newClient.City ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@createdAt", DateTime.Now);
            cmd.Parameters.AddWithValue("@updatedAt", DateTime.Now);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Удаление клиента из базы по Id
        /// </summary>
        /// <param name="clientId"></param>
        public static void DeleteClient(int clientId)
        {
            if (clientId == 0)
                return;

            using var connection = LogicDb.GetOpenConnection();

            string query = "DELETE FROM clients WHERE id = @id;";

            using var cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", clientId);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Обновление данных клиента
        /// </summary>
        /// <param name="client"></param>
        public static void UpdateClient(Client client)
        {
            if (client == null || client.Id == 0)
                return;

            using var connection = LogicDb.GetOpenConnection();

            string query = @"
                           UPDATE clients SET
                           full_name = @fullName,
                           opened_at = @openedAt,
                           status = @status,
                           notes = @notes,
                           phone = @phone,
                           email = @email,
                           telegram = @telegram,
                           city = @city,
                           updated_at = @updatedAt
                           WHERE id = @id;";

            using var cmd = new SQLiteCommand(query, connection);

            cmd.Parameters.AddWithValue("@fullName", client.FullName);
            cmd.Parameters.AddWithValue("@status", client.Status);
            cmd.Parameters.AddWithValue("@notes", client.Notes ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@phone", client.Phone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@email", client.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@telegram", client.Telegram ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@city", client.City ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@updatedAt", DateTime.Now);
            cmd.Parameters.AddWithValue("@id", client.Id);

            cmd.ExecuteNonQuery();

        }
    }
}
