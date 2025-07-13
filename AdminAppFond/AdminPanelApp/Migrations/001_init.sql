CREATE TABLE clients (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    full_name TEXT NOT NULL,
    opened_at TEXT NOT NULL,
    status TEXT NOT NULL DEFAULT 'active', -- active, blocked, closed
    notes TEXT,
    phone TEXT,
    email TEXT,
    telegram TEXT,
    city TEXT,
    created_at TEXT DEFAULT (datetime('now')),
    updated_at TEXT DEFAULT (datetime('now'))
);

CREATE TABLE messages (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    client_id INTEGER NOT NULL,
    sender TEXT NOT NULL, -- 'client' или 'admin'
    message TEXT NOT NULL,
    sent_at TEXT DEFAULT (datetime('now')),
    FOREIGN KEY (client_id) REFERENCES clients(id)
);

CREATE TABLE daily_trading_stats (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    client_id INTEGER NOT NULL,
    date TEXT NOT NULL,
    balance_start REAL,
    balance_end REAL,
    profit_loss REAL,
    created_at TEXT DEFAULT (datetime('now')),
    FOREIGN KEY (client_id) REFERENCES clients(id)
);


CREATE TABLE transactions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    client_id INTEGER NOT NULL,
    type TEXT NOT NULL, -- 'deposit', 'withdrawal', 'management_fee'
    amount REAL NOT NULL,
    status TEXT DEFAULT 'completed',
    processed_at TEXT,
    created_at TEXT DEFAULT (datetime('now')),
    FOREIGN KEY (client_id) REFERENCES clients(id)
);

CREATE TABLE accounts (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    client_id INTEGER NOT NULL,
    exchange TEXT NOT NULL,      -- Название биржи (например, Binance, Bybit)
    account_name TEXT NOT NULL,  -- Название счета или описание
    account_number TEXT UNIQUE NOT NULL,  -- Переносим из clients
    created_at TEXT DEFAULT (datetime('now')),
    FOREIGN KEY (client_id) REFERENCES clients(id)
);

CREATE TABLE IF NOT EXISTS __Migrations (
    Name TEXT PRIMARY KEY,
    AppliedAt TEXT NOT NULL
);
