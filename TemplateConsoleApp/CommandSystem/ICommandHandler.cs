using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TemplateConsoleApp.CommandSystem
{
    internal interface ICommandHandler
    {
        Task<TelegramBotCommand> Create(string commandName, CommandContext context);
        Task<Message> Run(Task<TelegramBotCommand> command);
    }
}
