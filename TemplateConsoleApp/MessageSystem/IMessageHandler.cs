using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TemplateConsoleApp.MessageSystem
{
    public interface IMessageHandler
    {
        Task<Message> CreateMessage(ITelegramBotClient botClient, CancellationToken token, Update update);
    }
}
