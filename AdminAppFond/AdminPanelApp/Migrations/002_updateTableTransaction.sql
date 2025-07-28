-- Удаляем старую таблицу transactions
DROP TABLE IF EXISTS transactions;

-- Создаем новую таблицу transactions с привязкой к accounts
CREATE TABLE transactions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    account_id INTEGER NOT NULL,
    type TEXT NOT NULL, -- 'deposit', 'withdrawal', 'management_fee'
    amount REAL NOT NULL,
    status TEXT DEFAULT 'completed',
    processed_at TEXT,
    created_at TEXT DEFAULT (datetime('now', 'utc')),
    FOREIGN KEY (account_id) REFERENCES accounts(id)
);

-- Записываем факт применения миграции
INSERT INTO __Migrations (Name, AppliedAt) VALUES ('2025_07_22_transactions_by_account', datetime('now'));