BEGIN TRANSACTION;

-- Добавляем новую колонку, если ее еще нет
ALTER TABLE clients ADD COLUMN chatid INTEGER;

-- Записываем факт применения миграции
INSERT INTO __Migrations (Name, AppliedAt) 
VALUES ('2025_08_05_add_chatid_to_clients', datetime('now'));

COMMIT;