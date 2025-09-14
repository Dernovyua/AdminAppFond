BEGIN TRANSACTION;

-- Добавляем новую колонку, если ее еще нет
ALTER TABLE clients ADD COLUMN success_fee REAL DEFAULT 0.0;

-- Записываем факт применения миграции
INSERT INTO __Migrations (Name, AppliedAt) 
VALUES ('2025_09_14_08_add_success_fee_client', datetime('now'));

COMMIT;