using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanelApp.Logic
{
    /// <summary>
    /// Логика взаимодействия с БД
    /// </summary>
    public static class LogicDb
    {
        static string _pathDb = "AdminDb.db";
        static string sqlInitFile = "001_init.sql";

        public static string ConnectionString => $"Data Source={_pathDb};Version=3;";

        private static void SetPath(string path)
        {
            _pathDb = Path.Combine(path, _pathDb);
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

            ApplyMigrations();
        }



        static void InitializeDatabase()
        {
            //var sqlFile = Path.Combine(AppContext.BaseDirectory, "Migrations", "001_init.sql");
            //var script = File.ReadAllText(sqlFile);
            //using var conn = new SQLiteConnection(ConnectionString);
            //conn.Open();
            //var cmd = new SQLiteCommand(script, conn);
            //cmd.ExecuteNonQuery();

            //MarkMigrationApplied(conn, sqlInitFile);
            //Console.WriteLine("База успешно инициализирована.");

            var resourceName = "AdminPanelApp.Migrations.001_init.sql";

            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)
                             ?? throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");

            using var reader = new StreamReader(stream);
            var script = reader.ReadToEnd();

            using var conn = new SQLiteConnection(ConnectionString);
            conn.Open();
            var cmd = new SQLiteCommand(script, conn);
            cmd.ExecuteNonQuery();

            MarkMigrationApplied(conn, sqlInitFile);
            Console.WriteLine("База успешно инициализирована.");
        }

        static void ApplyMigrations()
        {
            //var migrationDir = Path.Combine(AppContext.BaseDirectory, "Migrations");
            //if (!Directory.Exists(migrationDir))
            //{
            //    Console.WriteLine("Папка миграций не найдена.");
            //    return;
            //}

            using var conn = new SQLiteConnection(ConnectionString);
            conn.Open();

            EnsureMigrationsTable(conn);

            var applied = GetAppliedMigrations(conn);
            //var allFiles = Directory.GetFiles(migrationDir, "*.sql")
            //                        .OrderBy(f => f);

            //foreach (var file in allFiles)
            //{
            //    var name = Path.GetFileName(file);
            //    if (applied.Contains(name))
            //    {
            //        Console.WriteLine($"Пропущена миграция {name} (уже применена)");
            //        continue;
            //    }

            //    Console.WriteLine($"Применение миграции: {name}");
            //    var sql = File.ReadAllText(file);
            //    using var cmd = conn.CreateCommand();
            //    cmd.CommandText = sql;
            //    cmd.ExecuteNonQuery();

            //    MarkMigrationApplied(conn, name);
            //}

            var assembly = Assembly.GetExecutingAssembly();
            var resourcePrefix = "AdminPanelApp.Migrations."; 

            var allResources = assembly.GetManifestResourceNames()
                .Where(r => r.StartsWith(resourcePrefix) && r.EndsWith(".sql"))
                .OrderBy(r => r);

            foreach (var resourceName in allResources)
            {
                var name = resourceName.Substring(resourcePrefix.Length); // "001_init.sql"

                if (applied.Contains(name))
                {
                    Console.WriteLine($"Пропущена миграция {name} (уже применена)");
                    continue;
                }

                Console.WriteLine($"Применение миграции: {name}");

                using var stream = assembly.GetManifestResourceStream(resourceName)
                    ?? throw new InvalidOperationException($"Ресурс {resourceName} не найден");
                using var reader = new StreamReader(stream);
                var sql = reader.ReadToEnd();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();

                MarkMigrationApplied(conn, name);
            }

            Console.WriteLine("Все миграции применены.");
        }

        static string ReadEmbeddedSql(string resourceName)
        {
            var asm = Assembly.GetExecutingAssembly();
            using var stream = asm.GetManifestResourceStream(resourceName)
                ?? throw new FileNotFoundException($"Ресурс не найден: {resourceName}");
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        static void EnsureMigrationsTable(SQLiteConnection conn)
        {
            var sql = @"CREATE TABLE IF NOT EXISTS __Migrations (
                        Name TEXT PRIMARY KEY,
                        AppliedAt TEXT NOT NULL
                    );";
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        static HashSet<string> GetAppliedMigrations(SQLiteConnection conn)
        {
            var result = new HashSet<string>();
            var sql = "SELECT Name FROM __Migrations;";
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(reader.GetString(0));
            }
            return result;
        }

        static void MarkMigrationApplied(SQLiteConnection conn, string name)
        {
            var sql = "INSERT INTO __Migrations (Name, AppliedAt) VALUES (@name, @date);";
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@date", DateTime.UtcNow.ToString("u"));
            cmd.ExecuteNonQuery();
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
