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

    }
}
