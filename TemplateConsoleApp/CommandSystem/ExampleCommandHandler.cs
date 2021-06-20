using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TemplateConsoleApp.CommandSystem
{
    internal class ExampleCommandHandler : ICommandHandler
    {
        public async Task<TelegramBotCommand> Create(string commandName, CommandContext context)
        {
            return await Task.Run(() =>
            {
                TextInfo textInfo = new CultureInfo("en-us", false).TextInfo;
                var commandNameValid = textInfo.ToTitleCase(commandName)
                    .Replace("_", string.Empty)
                    .Replace(" ", string.Empty);
                Assembly assembly = Assembly.Load("TemplateConsoleApp");
                Type type = assembly.GetType("TemplateConsoleApp.CommandSystem." + commandNameValid + "Command");
                var commandConstructorArguments = new []{typeof(string), typeof(CommandContext)};
                var constructor = type.GetConstructor(commandConstructorArguments);
                var command = constructor?.Invoke(new object[] {commandName, context});
                if (command is TelegramBotCommand botCommand)
                {
                    return botCommand;
                }

                throw new MissingMethodException($"Command {commandName}");
            });
        }

        public Task<Message> Run(Task<TelegramBotCommand> command)
        {
            return command.Result.Execute();
        }
    }
}
