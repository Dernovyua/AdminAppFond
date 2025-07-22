-- Удаляем старую таблицу, если она существует
DROP TABLE IF EXISTS daily_trading_stats;

-- Создаём новую таблицу statistic
CREATE TABLE statistic (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    date TEXT NOT NULL,             -- Дата статистики
    deposit REAL NOT NULL,          -- Сумма депозита за день
    account_id INTEGER NOT NULL,    -- ID счёта (связь с accounts)
    comment TEXT,                   -- Комментарий
    created_at TEXT DEFAULT (datetime('now')),
    FOREIGN KEY (account_id) REFERENCES accounts(id)
);