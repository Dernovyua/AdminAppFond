using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Threading;


namespace AdminPanelApp.Logic
{
    public class TelegramBot
    {
        TelegramBotClient botClient;

        string _lastToken = "";

        public async Task CreateTgBot()
        {
            if (String.IsNullOrEmpty(LogicData.SettingCrm.TgTokenCrm) ||
                _lastToken== LogicData.SettingCrm.TgTokenCrm) //чтоб не создавался повторно при изменении настроек
                return;

            _lastToken = LogicData.SettingCrm.TgTokenCrm;

            botClient = new TelegramBotClient(LogicData.SettingCrm.TgTokenCrm);

            using var cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { } // получаем все обновления
            };

            botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                new ReceiverOptions
                {
                    AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery, }
                },
                cancellationToken: cts.Token
            );

            var me = await botClient.GetMe();
            //Console.WriteLine($"Бот запущен @{me.Username}");
            LogicData.RaiseOnSendMessage($"Бот запущен @{me.Username}");

            await Task.Delay(-1, cts.Token);
        }

        /// <summary>
        /// Логика работы с чатами
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            long chatId = 0;

            if (update.CallbackQuery != null)
            {
                var callbackData = update.CallbackQuery.Data;
                chatId = update.CallbackQuery.Message.Chat.Id;
                await botClient.AnswerCallbackQuery(update.CallbackQuery.Id);
            }

            if (update.Type != UpdateType.Message || update.Message!.Type != MessageType.Text)
                return;

            var message = update.Message;
            chatId = message.Chat.Id;

        }

        static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Ошибка: {exception.Message}");
            return Task.CompletedTask;
        }

    }
}
