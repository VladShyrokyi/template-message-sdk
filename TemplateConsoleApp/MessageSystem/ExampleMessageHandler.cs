using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TemplateLib;
using TemplateLib.Factory;

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

            var headBlock = TextBlockFactory.CreateSimpleEmptyWith(
                $"User: {DefaultRegex.CreateSelector(user)}{DefaultRegex.CreateSelector(append)}"
            );
            headBlock.PutVariable(user, update.Message.Chat.Username);
            headBlock.PutVariable(append, $" ({update.Message.Chat.LastName} {update.Message.Chat.FirstName})");

            var bodyBlock = TextBlockFactory.CreateSimpleEmptyWith(
                $"Text: {DefaultRegex.CreateSelector(text)}{DefaultRegex.CreateSelector(append)}"
            );
            bodyBlock.PutVariable(text, update.Message.Text);

            var bottomBlock = TextBlockFactory.CreateSimpleEmptyWith(
                $"Time: {DefaultRegex.CreateSelector(time)}{DefaultRegex.CreateSelector(append)}"
            );
            bottomBlock.PutVariable(time, update.Message.Date.ToString(CultureInfo.CurrentCulture));

            var block = TextBlockFactory.CreateTemplateWith("\n", headBlock, bodyBlock, bottomBlock);

            return botClient.SendTextMessageAsync(update.Message.Chat, block.Write(), cancellationToken: token);
        }
    }
}