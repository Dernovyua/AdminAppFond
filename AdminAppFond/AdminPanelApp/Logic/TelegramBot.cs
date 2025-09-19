using AdminPanelApp.Models;
using AdminPanelApp.Models.Scenario;
using AdminPanelApp.Requests;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
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
            LogicData.RaiseOnSendMessage("Сообщение из чата: " + update?.Message?.Chat.Id);

            if (update.Message?.Text?.ToString().ToUpper() == "#ID" ||
                update.Message?.Text?.ToString() == "/start")
            {
                var keyboard = SetMenu("");

                await botClient.SendMessage(
                            chatId: update.Message.Chat.Id,
                            text: $"Для начала работы, передайте ваш ID = {update.Message.Chat.Id} через личный чат человеку приславшему ссылку на этот бот",
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

        public void SendMeessageToUserFromAdmin(long chatId, string message, ReplyKeyboardMarkup menu, string pathToDocument)
        {
            _messageToUser.Enqueue(new MessageAdmin { ChatId = chatId, Message = message, Menu = menu, PathToDocument=  pathToDocument });
        }

        static ConcurrentQueue<Update> _messageFromUser = new ConcurrentQueue<Update>();
        static ConcurrentQueue<MessageAdmin> _messageToUser = new ConcurrentQueue<MessageAdmin>();


        public class MessageAdmin
        {
            public long ChatId { get; set; }
            public string Message { get; set; }
            public ReplyKeyboardMarkup Menu { get; set; }
            public string PathToDocument { get; set; }
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
                                //LogicData.RaiseOnSendMessage("Сообщение из чата: " + chatId);
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

                                    //LogicData.RaiseOnSendMessage("Сообщение от клиента чат: " + client.ChatId);
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
                            //LogicData.RaiseOnSendMessage("Проверка: " + mes.ChatId);
                            // Find client - consider using a dictionary for O(1) lookups
                            var client = LogicData.Clients.FirstOrDefault(c => c.ChatId == mes.ChatId);
                            if (client != null)
                            {

                                var text = !string.IsNullOrEmpty(mes.Message) ? mes.Message : "Выберите действие:";
                                //if (mes.Menu != null)
                                if (String.IsNullOrEmpty(mes.PathToDocument))
                                {
                                    await botClient.SendMessage(
                                          chatId: mes.ChatId,
                                          text: text,
                                          replyMarkup: mes.Menu,
                                          parseMode: ParseMode.Html);
                                }
                                else
                                {
                                    // Отправка документа
                                    await botClient.SendDocument(
                                        chatId: mes.ChatId,
                                        document: InputFile.FromStream(
                                                    File.OpenRead(mes.PathToDocument),
                                                    Path.GetFileName(mes.PathToDocument)
                                                ),
                                        caption: mes.Message, // подпись к файлу
                                        parseMode: ParseMode.Html,
                                        replyMarkup: mes.Menu
                                    );
                                }

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


                bool isMenu = false;
                ReplyKeyboardMarkup menuKey = null;
                string pathToDocument="";
                if (menuItem == _menuStatistic)
                {
                    //var client = LogicData.StatisticDisplay.Where(c => c.ClientName.ChatId == chatId).ToList();
                    var client = LogicData.Clients.Where(c => c.ChatId == chatId).FirstOrDefault();
                    if (client == null)
                        return;

                    message = GetTelegramStatsMessage(client);
                    isMenu = true;
                }
                else
                {
                    if (menuItem == "Назад")
                    {
                        menuKey=SetMenu("");
                        isMenu = true;
                    }
                    else
                        for (int i = LogicData.TgMenus.Count - 1; i >= 0; i--)
                        {
                            if (LogicData.TgMenus[i] is TgMenuModel menu)
                            {
                                if (menu.Name == menuItem)
                                {
                                    isMenu = true;
                                    if (!String.IsNullOrEmpty(menu.Text))
                                        message = menu.Text;
                                    pathToDocument = menu.PathToDocument;
                                    //if (menu.Level != "Главное меню")
                                    menuKey = SetMenu(menu.Name);
                                }
                            }
                        }
                }

                if (isMenu )//!String.IsNullOrEmpty(message))
                    SendMeessageToUserFromAdmin(chatId, message, menuKey, pathToDocument);
            }
            catch (Exception ex)
            {
                LogicData.RaiseOnSendMessage("Ошибка. Обработки меню бота: " + ex.Message);
            }
        }


        public string GetTelegramStatsMessage(Client client)
        {
            var sb = new StringBuilder();

            foreach (var acc in client.Accounts)
            {
                sb.AppendLine($"💰 Счет: {acc.AccountName}");
                sb.AppendLine($"💵 Текущий баланс: {acc.StatResult.Balance:N2} $");
                sb.AppendLine("Доходность за период:");
                sb.AppendLine(FormatReturn(acc.StatResult.TotalReturn, "За всё время"));
                sb.AppendLine(FormatReturn(acc.StatResult.AnnualReturn, "1 год"));
                sb.AppendLine(FormatReturn(acc.StatResult.Return6Months, "6 месяцев"));
                sb.AppendLine(FormatReturn(acc.StatResult.Return3Months, "3 месяца"));
                sb.AppendLine(FormatReturn(acc.StatResult.Return1Month, "1 месяц"));
                sb.AppendLine(FormatReturn(acc.StatResult.Return1Week, "1 неделя"));
                if (acc != client.Accounts.Last())
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

        /// <summary>
        /// Собираем меню. Кнопка статистика по умолчанию
        /// </summary>
        private ReplyKeyboardMarkup SetMenu(string level)
        {
            List<List<TgMenuModel>> menuModels = new List<List<TgMenuModel>>();
            foreach (var menu in LogicData.TgMenus)
            {
                if (String.IsNullOrEmpty(level) && menu.Level == "Главное меню" ||
                    menu.Level == level)
                {
                    while (menuModels.Count <= menu.Row)
                    {
                        menuModels.Add(new List<TgMenuModel>());
                    }
                    while (menuModels[menu.Row].Count <= menu.Column)
                    {
                        menuModels[menu.Row].Add(null);
                    }
                    menuModels[menu.Row][menu.Column] = menu;
                }
            }

            // Создаем список строк для клавиатуры
            var keyboardRows = new List<KeyboardButton[]>();

            // Добавляем не-null элементы построчно
            foreach (var row in menuModels)
            {
                // Фильтруем null значения и создаем кнопки
                var buttons = row
                    .Where(menu => menu != null && menu.IsRun)
                    .Select(menu => new KeyboardButton(menu.Name))
                    .ToArray();

                if (buttons.Length > 0)
                {
                    keyboardRows.Add(buttons);
                }
            }
            if (menuModels.Count == 0 && !String.IsNullOrEmpty(level))
                return null;

            if (String.IsNullOrEmpty(level))
            {
                // Добавляем кнопку статистики в самый конец
                keyboardRows.Add(new[] { new KeyboardButton(_menuStatistic) });
            }
            else
            {
                keyboardRows.Add(new[] { new KeyboardButton("Назад") });
            }

            // Создаем клавиатуру
            var keyboard = new ReplyKeyboardMarkup(keyboardRows)
            {
                ResizeKeyboard = true // Делает кнопки компактнее
            };

            return keyboard;
        }


    }
}
