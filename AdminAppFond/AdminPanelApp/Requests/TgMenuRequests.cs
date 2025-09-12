using AdminPanelApp.Logic;
using AdminPanelApp.Models.Scenario;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanelApp.Requests
{
    public static class TgMenuRequests
    {
        /// <summary>
        /// Обновление элемента меню
        /// </summary>
        /// <param name="updatedMenu"></param>
        public static void UpdateTgMenu(TgMenuModel updatedMenu)
        {
            using var conn = LogicDb.GetOpenConnection();

            using var cmd = new SQLiteCommand(conn);
            cmd.CommandText = @"
        UPDATE tg_menu SET
            is_run = @isRun,
            level = @level,
            name = @name,
            column = @column,
            row = @row,
            text_to_user = @textToUser,
            path_to_document = @pathToDocument
        WHERE id = @id;
    ";

            cmd.Parameters.AddWithValue("@isRun", updatedMenu.IsRun);
            cmd.Parameters.AddWithValue("@level", updatedMenu.Level);
            cmd.Parameters.AddWithValue("@name", updatedMenu.Name);
            cmd.Parameters.AddWithValue("@column", updatedMenu.Column);
            cmd.Parameters.AddWithValue("@row", updatedMenu.Row);
            cmd.Parameters.AddWithValue("@textToUser", updatedMenu.Text);
            cmd.Parameters.AddWithValue("@pathToDocument", updatedMenu.PathToDocument);
            cmd.Parameters.AddWithValue("@id", updatedMenu.Id);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Добавление нового элемента меню в базу
        /// </summary>
        /// <param name="newMenu"></param>
        public static void AddTgMenu(TgMenuModel newMenu)
        {
            using var conn = LogicDb.GetOpenConnection();

            using var cmd = new SQLiteCommand(conn);
            cmd.CommandText = @"
        INSERT INTO tg_menu (is_run, level, name, column, row, text_to_user, path_to_document)
        VALUES (@isRun, @level, @name, @column, @row, @textToUser, @pathToDocument);
    ";

            cmd.Parameters.AddWithValue("@isRun", newMenu.IsRun);
            cmd.Parameters.AddWithValue("@level", newMenu.Level);
            cmd.Parameters.AddWithValue("@name", newMenu.Name);
            cmd.Parameters.AddWithValue("@column", newMenu.Column);
            cmd.Parameters.AddWithValue("@row", newMenu.Row);
            cmd.Parameters.AddWithValue("@textToUser", newMenu.Text);
            cmd.Parameters.AddWithValue("@pathToDocument", newMenu.PathToDocument);

            cmd.ExecuteNonQuery();

            // Получаем Id последней вставленной записи
            cmd.CommandText = "SELECT last_insert_rowid();";
            long lastId = (long)cmd.ExecuteScalar();

            newMenu.Id = (int)lastId; // присваиваем Id элементу меню
        }

        /// <summary>
        /// Удаление элемента меню из базы по Id
        /// </summary>
        /// <param name="menuId"></param>
        public static void DeleteTgMenu(int menuId)
        {
            using var conn = LogicDb.GetOpenConnection();

            using var cmd = new SQLiteCommand(conn);
            cmd.CommandText = "DELETE FROM tg_menu WHERE id = @id;";
            cmd.Parameters.AddWithValue("@id", menuId);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Получение всех элементов меню из базы
        /// </summary>
        /// <returns></returns>
        public static List<TgMenuModel> GetAllTgMenus()
        {
            var menus = new List<TgMenuModel>();

            using var conn = LogicDb.GetOpenConnection();

            using var cmd = new SQLiteCommand(conn);
            cmd.CommandText = "SELECT * FROM tg_menu ORDER BY level, column, row;";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                menus.Add(new TgMenuModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    IsRun = reader.GetBoolean(reader.GetOrdinal("is_run")),
                    Level = reader.IsDBNull(reader.GetOrdinal("level")) ? null : reader.GetString(reader.GetOrdinal("level")),
                    Name = reader.IsDBNull(reader.GetOrdinal("name")) ? null : reader.GetString(reader.GetOrdinal("name")),
                    Column = reader.GetInt32(reader.GetOrdinal("column")),
                    Row = reader.GetInt32(reader.GetOrdinal("row")),
                    Text = reader.IsDBNull(reader.GetOrdinal("text_to_user")) ? null : reader.GetString(reader.GetOrdinal("text_to_user")),
                    PathToDocument = reader.IsDBNull(reader.GetOrdinal("path_to_document")) ? null : reader.GetString(reader.GetOrdinal("path_to_document"))
                });
            }

            return menus;
        }
    }
}
