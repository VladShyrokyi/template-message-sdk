using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TemplateConsoleApp.CommandSystem
{
    internal class CommandContext
    {
        public ITelegramBotClient BotClient;
        public CancellationToken Token;
        public Update Update;

        public CommandContext(ITelegramBotClient botClient, CancellationToken token, Update update)
        {
            BotClient = botClient;
            Token = token;
            Update = update;
        }
    }
}
