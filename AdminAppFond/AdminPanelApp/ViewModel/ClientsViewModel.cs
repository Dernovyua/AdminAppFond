using AdminPanelApp.Logic;
using AdminPanelApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanelApp.ViewModel
{
    public class ClientsViewModel
    {
        public ClientsViewModel()
        { }
        public ClientsViewModel(ObservableCollection<Client> clients)
        {
            Clients = clients;
        }

        private readonly string _connectionString = "Data Source=clients.db;Version=3;";
        public ObservableCollection<Client> Clients { get; set; } = new();

        public void AddClient(Client newClient)
        {
            using var connection = LogicDb.GetOpenConnection();

            string query = @"
                INSERT INTO clients (full_name, account_number, opened_at, status, notes)
                VALUES (@FullName, @AccountNumber, @OpenedAt, @Status, @Notes);
                SELECT last_insert_rowid();
            ";

            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@FullName", newClient.FullName);
            command.Parameters.AddWithValue("@AccountNumber", newClient.AccountNumber);
            command.Parameters.AddWithValue("@OpenedAt", newClient.OpenedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@Status", newClient.Status);
            command.Parameters.AddWithValue("@Notes", newClient.Notes ?? string.Empty);

            long id = (long)command.ExecuteScalar();
            newClient.Id = (int)id;
            newClient.CreatedAt = DateTime.Now;
            newClient.UpdatedAt = DateTime.Now;

            Clients.Add(newClient);

        }


        public void DeleteClient(Client client)
        {
            if (client == null || client.Id == 0)
                return;

            using var connection = LogicDb.GetOpenConnection();

            string query = "DELETE FROM clients WHERE id = @Id";

            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@Id", client.Id);

            int affectedRows = command.ExecuteNonQuery();

            if (affectedRows > 0)
            {
                Clients.Remove(client);
            }
        }

        public void UpdateClient(Client client)
        {
            if (client == null || client.Id == 0)
                return;

            using var connection = LogicDb.GetOpenConnection();

            string query = @"
                            UPDATE clients 
                            SET full_name = @FullName,
                                account_number = @AccountNumber,
                                opened_at = @OpenedAt,
                                status = @Status,
                                notes = @Notes,
                                updated_at = @UpdatedAt
                            WHERE id = @Id";

            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@FullName", client.FullName);
            command.Parameters.AddWithValue("@AccountNumber", client.AccountNumber);
            command.Parameters.AddWithValue("@OpenedAt", client.OpenedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@Status", client.Status);
            command.Parameters.AddWithValue("@Notes", client.Notes ?? string.Empty);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@Id", client.Id);

            command.ExecuteNonQuery();

            // Можно также обновить UpdatedAt в объекте клиента
            client.UpdatedAt = DateTime.Now;
        }

    }
}
