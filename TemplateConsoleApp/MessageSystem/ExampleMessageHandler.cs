using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Types;

using TemplateLib;
using TemplateLib.Block;
using TemplateLib.Factory;

namespace TemplateConsoleApp.MessageSystem
{
    internal class ExampleMessageHandler : IMessageHandler
    {
        public Task<Message> CreateMessage(ITelegramBotClient botClient, CancellationToken token, Update update)
        {
            const string user = "User";
            const string text = "Text";
            const string time = "Time";

            var block = TextBlockFactory.MergeTemplates(
                "\n",
                CreateField(user, update.Message.Chat.Username,
                    $" ({update.Message.Chat.LastName} {update.Message.Chat.FirstName})"),
                CreateField(text, update.Message.Text),
                CreateField(time, update.Message.Date.ToString(CultureInfo.CurrentCulture))
            );
            return botClient.SendTextMessageAsync(update.Message.Chat, block.Write(),
                cancellationToken: token);
        }

        private static ITextBlock CreateField(string name, string text, string append = "")
        {
            const string appendSelector = "APPEND";
            var nameSelector = name.ToUpper();

            return TextBlockFactory.CreateTemplate(
                $"{name}: {DefaultRegex.SelectorFactory.Invoke(nameSelector)}{DefaultRegex.SelectorFactory.Invoke(appendSelector)}",
                new Dictionary<string, ITextBlock>
                {
                    {nameSelector, TextBlockFactory.CreateText(text)},
                    {appendSelector, TextBlockFactory.CreateText(append)}
                }
            );
        }
    }
}
