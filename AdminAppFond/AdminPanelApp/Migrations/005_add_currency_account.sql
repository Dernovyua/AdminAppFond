BEGIN TRANSACTION;

-- Добавляем новую колонку, если ее еще нет
ALTER TABLE accounts ADD COLUMN currency TEXT;

-- Записываем факт применения миграции
INSERT INTO __Migrations (Name, AppliedAt) 
VALUES ('2025_07_28_add_currency_to_accounts', datetime('now'));

COMMIT;