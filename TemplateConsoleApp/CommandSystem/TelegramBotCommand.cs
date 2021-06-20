using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TemplateConsoleApp.CommandSystem
{
    internal abstract class TelegramBotCommand
    {
        public string Name { get; }
        public DateTime Time { get; }
        public ITelegramBotClient BotClient { get; }
        public CancellationToken Token { get; }
        public Update Update { get; }

        protected TelegramBotCommand(string name, CommandContext context)
        {
            Name = name;
            BotClient = context.BotClient;
            Token = context.Token;
            Update = context.Update;
            Time = DateTime.Now;
        }

        public abstract Task<Message> Execute();
    }
}
