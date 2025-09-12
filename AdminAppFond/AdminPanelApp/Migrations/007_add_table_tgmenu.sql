CREATE TABLE IF NOT EXISTS tg_menu (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    is_run BOOLEAN NOT NULL DEFAULT 0,
    level TEXT,
    name TEXT,
    column INTEGER NOT NULL DEFAULT 0,
    row INTEGER NOT NULL DEFAULT 0,
    text_to_user TEXT,
    path_to_document TEXT
);

-- Записываем факт применения миграции
INSERT INTO __Migrations (Name, AppliedAt) 
VALUES ('12_09_2025_007_add_table_tg_menu', datetime('now'));
