using AdminPanelApp.Models;
using AdminPanelApp.Requests;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


namespace AdminPanelApp.Logic
{
    public class TelegramBot
    {
        TelegramBotClient botClient;

        string _lastToken = "";

        public async Task CreateTgBot()
        {
            if (String.IsNullOrEmpty(LogicData.SettingCrm.TgTokenCrm) ||
                _lastToken == LogicData.SettingCrm.TgTokenCrm) //чтоб не создавался повторно при изменении настроек
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

            CheckMessageFromUser();
            CheckMessageToUser();

            await Task.Delay(-1, cts.Token);

        }


        /// <summary>
        /// Логика работы с чатами
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            if (update.Type != UpdateType.Message || update.Message!.Type != MessageType.Text)
                return;

            if (update.Message?.Text?.ToString().ToUpper() == "#ID" ||
                update.Message?.Text?.ToString() == "/start")
            {
                //var inlineKeyboard = new InlineKeyboardMarkup(new[]
                //                    {
                //                            new[] { InlineKeyboardButton.WithCallbackData("Статистика", "stats_btn") }
                //                        });

                var keyboard = new ReplyKeyboardMarkup(new[]
                                {
                                new[] { new KeyboardButton(_menuStatistic) }
                                })
                {
                    ResizeKeyboard = true // Делает кнопки компактнее
                };

                await botClient.SendMessage(
                            chatId: update.Message.Chat.Id,
                            text: $"Ваш ID = {update.Message.Chat.Id}. Передайте его администратору через личный чат",
                            cancellationToken: cancellationToken,
                            replyMarkup: keyboard);
            }



            _messageFromUser.Enqueue(update);
        }

        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Ошибка: {exception.Message}");
            return Task.CompletedTask;
        }

        public void SendMeessageToUserFromAdmin(long chatId, string message)
        {
            _messageToUser.Enqueue(new MessageAdmin { ChatId = chatId, Message = message });
        }

        static ConcurrentQueue<Update> _messageFromUser = new ConcurrentQueue<Update>();
        static ConcurrentQueue<MessageAdmin> _messageToUser = new ConcurrentQueue<MessageAdmin>();


        public class MessageAdmin
        {
            public long ChatId { get; set; }
            public string Message { get; set; }
        }

        public async Task CheckMessageFromUser()
        {
            try
            {
                while (true)
                {
                    while (!_messageFromUser.IsEmpty)
                    {
                        if (_messageFromUser.TryDequeue(out var upd))
                        {
                            if (upd?.Message?.Chat.Id is long chatId)
                            {
                                // Find client - consider using a dictionary for O(1) lookups
                                var client = LogicData.Clients.FirstOrDefault(c => c.ChatId == chatId);
                                if (client != null)
                                {

                                    // Dispatch to UI thread if needed
                                    await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                                    {
                                        client.Chat.IsUnread = true;
                                        client.Chat.LastMessageTime = upd.Message.Date;
                                        client.Chat.LastMessage = upd.Message.Text;
                                        var model = new Models.MessageItemModel
                                        {
                                            SentAt = upd.Message.Date,
                                            Text = upd.Message.Text
                                        };
                                        client.Chat.Messages.Add(model);

                                        MessagesRequests.AddMessage(model, client.Id);
                                    });

                                    CheckMenu(client.ChatId, upd.Message.Text);
                                }
                            }
                        }
                    }
                    await Task.Delay(1);
                }
            }
            catch (Exception ex)
            {
                LogicData.RaiseOnSendMessage("Ошибка. Входящие сообщение от бота: " + ex.Message);
            }
        }


        public async Task CheckMessageToUser()
        {

            while (true)
            {
                while (!_messageToUser.IsEmpty)
                {
                    try
                    {
                        if (_messageToUser.TryDequeue(out var mes))
                        {
                            // Find client - consider using a dictionary for O(1) lookups
                            var client = LogicData.Clients.FirstOrDefault(c => c.ChatId == mes.ChatId);
                            if (client != null)
                            {
                                await botClient.SendMessage(
                                    chatId: mes.ChatId,
                                    text: mes.Message);

                                // Dispatch to UI thread if needed
                                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                                {
                                    client.Chat.LastMessageTime = DateTime.Now;
                                    client.Chat.LastMessage = mes.Message;
                                    var model = new Models.MessageItemModel
                                    {
                                        SentAt = DateTime.Now,
                                        Text = mes.Message,
                                        IsOwn = true
                                    };
                                    client.Chat.Messages.Add(model);

                                    MessagesRequests.AddMessage(model, client.Id);
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogicData.RaiseOnSendMessage("Ошибка. Исходящего сообщения от бота: " + ex.Message);
                    }
                }

                await Task.Delay(1);
            }

        }


        string _menuStatistic = "Статистика";

        private void CheckMenu(long chatId, string menuItem)
        {
            try
            {
                string message = "";
                var client = LogicData.StatisticDisplay.Where(c => c.ClientName.ChatId == chatId).ToList();
                if (client.Count == 0)
                    return;

                if (menuItem == _menuStatistic)
                {
                    message = GetTelegramStatsMessage(client);
                }

                if (!String.IsNullOrEmpty(message))
                    SendMeessageToUserFromAdmin(chatId, message);
            }
            catch (Exception ex)
            {
                LogicData.RaiseOnSendMessage("Ошибка. Обработки меню бота: " + ex.Message);
            }
        }


        public string GetTelegramStatsMessage(List<StatisticDisplayModel> stats)
        {
            var sb = new StringBuilder();

            foreach (var stat in stats)
            {
                sb.AppendLine($"💰 Счет: {stat.Account.AccountName}");
                sb.AppendLine($"💵 Текущий баланс: {stat.Balance:N2} $");
                sb.AppendLine("Доходность:");
                sb.AppendLine(FormatReturn(stat.TotalReturn, "За всё время"));
                sb.AppendLine(FormatReturn(stat.AnnualReturn, "Годовая"));
                sb.AppendLine(FormatReturn(stat.Return6Months, "6 месяцев"));
                sb.AppendLine(FormatReturn(stat.Return3Months, "3 месяца"));
                sb.AppendLine(FormatReturn(stat.Return1Month, "1 месяц"));
                sb.AppendLine(FormatReturn(stat.Return1Week, "1 неделя"));
                if (stat != stats.Last())
                    sb.AppendLine();
            }

            return sb.ToString();
        }

        // Функция для форматирования с цветом
        string FormatReturn(decimal value, string period)
        {
            if (value > 0)
                return $"🟢 {period}: +{value:N2}%";
            if (value < 0)
                return $"🔴 {period}: {value:N2}%";
            return $"⚪️ {period}: {value:N2}%";
        }
    }
}
