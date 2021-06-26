using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TemplateLib.Factory;
using TemplateLib.Models;

namespace TemplateConsoleApp.MessageSystem
{
    internal class ExampleMessageHandler : IMessageHandler
    {
        public Task<Message> CreateMessage(ITelegramBotClient botClient, CancellationToken token, Update update)
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

            var block = TextBlockFactory.MergeText("DYN", "\n", headBlock, bodyBlock, bottomBlock);

            return botClient.SendTextMessageAsync(update.Message.Chat, block.WriteWithEditor(), cancellationToken: token);
        }
    }
}
