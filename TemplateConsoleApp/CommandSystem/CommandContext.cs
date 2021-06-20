using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TemplateConsoleApp.CommandSystem
{
    internal class CommandContext
    {
        public readonly ITelegramBotClient BotClient;
        public readonly CancellationToken Token;
        public readonly Update Update;

        public CommandContext(ITelegramBotClient botClient, CancellationToken token, Update update)
        {
            BotClient = botClient;
            Token = token;
            Update = update;
        }
    }
}
