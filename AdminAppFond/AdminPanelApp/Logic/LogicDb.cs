using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanelApp.Logic
{
    /// <summary>
    /// Логика взаимодействия с БД
    /// </summary>
    public static class LogicDb
    {
        static string _pathDb = "prop_admin.db";
        static string sqlInitFile = "init.sql";

        public static string ConnectionString => $"Data Source={_pathDb};Version=3;";

        private static void SetPath(string path)
        {
            _pathDb = Path.Combine(path, "prop_admin.db");
        }

        public static void Main(string path)
        {
            SetPath(path);

            if (!File.Exists(_pathDb))
            {
                Console.WriteLine("Создание базы данных...");
                Directory.CreateDirectory(Path.GetDirectoryName(_pathDb));
                SQLiteConnection.CreateFile(_pathDb);
                InitializeDatabase();
            }
            else
            {
                Console.WriteLine("База данных уже существует.");
            }
        }



        static void InitializeDatabase()
        {
            var script = File.ReadAllText(sqlInitFile);
            using var conn = new SQLiteConnection(ConnectionString);
            conn.Open();
            var cmd = new SQLiteCommand(script, conn);
            cmd.ExecuteNonQuery();
            Console.WriteLine("База успешно инициализирована.");
        }


        /// <summary>
        /// Получить открытое соединение с БД
        /// </summary>
        public static SQLiteConnection GetOpenConnection()
        {
            var connection = new SQLiteConnection(ConnectionString);
            connection.Open();
            return connection;
        }
    }



}
