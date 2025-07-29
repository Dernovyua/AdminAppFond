BEGIN TRANSACTION;

-- Добавляем новую колонку, если ее еще нет
ALTER TABLE transactions ADD COLUMN comment TEXT;

-- Записываем факт применения миграции
INSERT INTO __Migrations (Name, AppliedAt) 
VALUES ('2025_07_28_add_comment_to_transactions', datetime('now'));

COMMIT;