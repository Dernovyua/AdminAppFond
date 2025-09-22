BEGIN TRANSACTION;

-- Добавляем новую колонку, если ее еще нет
ALTER TABLE transactions ADD COLUMN paid REAL DEFAULT 0;

-- Записываем факт применения миграции
INSERT INTO __Migrations (Name, AppliedAt) 
VALUES ('2025_09_22_add_paid_to_transactions', datetime('now'));

COMMIT;