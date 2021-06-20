using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using TemplateLib.Factory;
using TemplateLib.Objects;

namespace TemplateConsoleApp
{
    internal static class Program
    {
        private const string Token = "";
        private const string ExceptionChatId = "";
        private static readonly ITelegramBotClient BotClient = new TelegramBotClient(Token);

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
                await MessageHandler(botClient, token, update);
            }
        }

        private static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken token)
        {
            if (exception is ApiRequestException apiRequestException)
            {
                await botClient.SendTextMessageAsync(ExceptionChatId, apiRequestException.ToString(), cancellationToken: token);
            }
        }

        private static Task<Message> MessageHandler(ITelegramBotClient botClient, CancellationToken token, Update update)
        {
            const string user = "USER";
            const string text = "TEXT";
            const string time = "TIME";
            const string append = "APPEND";

            var headBlock = new TextBlock($"User: %[{user}]%%[{append}]%");
            headBlock.PutVariable(user, update.Message.Chat.Username);
            headBlock.PutVariable(append, $" ({update.Message.Chat.LastName} {update.Message.Chat.FirstName})");

            var bodyBlock = new TextBlock($"Text: %[{text}]%%[{append}]%");
            bodyBlock.PutVariable(text, update.Message.Text);

            var bottomBlock = new TextBlock($"Time: %[{time}]%%[{append}]%");
            bottomBlock.PutVariable(time, update.Message.Date.ToString(CultureInfo.CurrentCulture));

            var block = TextBlockFactory.CreateText("DYN", "\n", headBlock, bodyBlock, bottomBlock);

            return botClient.SendTextMessageAsync(update.Message.Chat, block.WriteWithEditor(), cancellationToken: token);
        }
    }
}
