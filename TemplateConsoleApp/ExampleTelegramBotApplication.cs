using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using TemplateConsoleApp.CommandSystem;
using TemplateConsoleApp.MessageSystem;

namespace TemplateConsoleApp
{
    internal static class ExampleTelegramBotApplication
    {
        private const string Token = "";
        private const string ExceptionChatId = "";
        private static readonly ITelegramBotClient BotClient = new TelegramBotClient(Token);
        private static readonly IMessageHandler MessageHandler = new ExampleMessageHandler();
        private static readonly ICommandHandler CommandHandler = new ExampleCommandHandler();

        public static async Task Main(string[] args)
        {
            var me = await BotClient.GetMeAsync();
            Console.WriteLine($"Hello, i'm user {me.Id}m name {me.Username}");
            Console.Title = me.Username;

            var cts = new CancellationTokenSource();

            await BotClient.ReceiveAsync(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), cts.Token);

            Console.WriteLine("Await...");
            Console.ReadLine();

            cts.Cancel();
        }

        private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            if (update.Message != null && update.Message is var message)
            {
                Console.WriteLine($"Have message from request:");
                Console.WriteLine($"        Id - {update.Id}");
                Console.WriteLine($"        Type - {update.Type}");
                Console.WriteLine($"        Chat - {message.Chat.Username}({message.Chat.LastName} {message.Chat.FirstName})");
                Console.WriteLine($"        Data - {message.Date}");
                Console.WriteLine($"        Text - {message.Text}");
                if (message.Text[0].Equals('/'))
                {
                    var command = CommandHandler.Create(
                        message.Text.Substring(1),
                        new CommandContext(botClient, token, update)
                    );
                    await CommandHandler.Run(command);
                }
                else
                {
                    await MessageHandler.CreateMessage(botClient, token, update);
                }
            }
        }

        private static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken token)
        {
            if (exception is ApiRequestException apiRequestException)
            {
                await botClient.SendTextMessageAsync(ExceptionChatId, apiRequestException.ToString(), cancellationToken: token);
            }
        }
    }
}
