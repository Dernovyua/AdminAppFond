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
    public class MessagesRequests
    {

        // <param name="message">Модель сообщения</param>
        /// <param name="clientId">ID клиента, с которым связано сообщение</param>
        public static void AddMessage(MessageItemModel message, int clientId)
        {
            if (message == null)
                return;

            using var connection = LogicDb.GetOpenConnection();

            string query = @"
                    INSERT INTO messages (client_id, sender, message, sent_at)
                    VALUES (@clientId, @sender, @message, @sentAt);
                ";

            using var cmd = new SQLiteCommand(query, connection);

            cmd.Parameters.AddWithValue("@clientId", clientId);
            cmd.Parameters.AddWithValue("@sender", message.IsOwn ? "admin" : "client");
            cmd.Parameters.AddWithValue("@message", message.Text);
            cmd.Parameters.AddWithValue("@sentAt", message.SentAt.ToUniversalTime());

            cmd.ExecuteNonQuery();

            // Если нужно получить ID вставленного сообщения
            cmd.CommandText = "SELECT last_insert_rowid();";
            long lastId = (long)cmd.ExecuteScalar();

        }

        /// <summary>
        /// Загрузка сообщений для конкретного клиента
        /// </summary>
        public static void LoadClientMessages(Client client)
        {
            if (client == null || client.ChatId==0) return;

            using var connection = LogicDb.GetOpenConnection();

            string query = "SELECT * FROM messages WHERE client_id = @clientId ORDER BY sent_at ASC";
            using var cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddWithValue("@clientId", client.Id);
            using var reader = cmd.ExecuteReader();

            var messages = new ObservableCollection<MessageItemModel>();

            while (reader.Read())
            {
                messages.Add(new MessageItemModel
                {
                    Text = reader.GetString(reader.GetOrdinal("message")),
                    SentAt = reader.GetDateTime(reader.GetOrdinal("sent_at")),
                    IsOwn = reader.GetString(reader.GetOrdinal("sender")) == "admin"
                });
            }

            client.Chat.Messages = messages;

            if (messages.Count > 0)
            {
                var lastMessage = messages.Last();
                client.Chat.LastMessage = lastMessage.Text;
                client.Chat.LastMessageTime = lastMessage.SentAt;
            }
        }
    }
}
