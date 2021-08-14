using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using TemplateLib;
using TemplateLib.Block;
using TemplateLib.Builder;
using TemplateLib.Checker;
using TemplateLib.Editor;
using TemplateLib.Factory;

namespace TemplateConsoleApp.CommandSystem
{
    internal class GetJsonCommand : TelegramBotCommand
    {
        private const string Url = "https://jsonplaceholder.typicode.com/posts";
        private const int MaxCharCount = 4096;
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly JsonSerializer _jsonSerializer = JsonSerializer.CreateDefault();

        public GetJsonCommand(string name, CommandContext context) : base(name, context) { }

        public override Task<Message> Execute()
        {
            const string title = "TITLE";
            const string body = "BODY";

            var charCountChecker = new CharCountChecker(MaxCharCount);
            var builder = new ConditionBlockBuilder("", DefaultRegex.DynamicVariableName, charCountChecker);

            var titleBlock =
                TextBlockFactory.CreateTemplate($"Response from {DefaultRegex.SelectorFactory.Invoke(title)}",
                    new Dictionary<string, ITextBlock>
                    {
                        {title, TextBlockFactory.CreateText(Url)}
                    });
            titleBlock.Editor = new BoldEditor();
            builder.Append(titleBlock);

            var data = GetData();
            if (data == null)
                return BotClient.SendTextMessageAsync(
                    Update.Message.Chat,
                    builder.Build().Write(),
                    ParseMode.Html,
                    cancellationToken: Token
                );

            builder.Append(TextBlockFactory.CreateText("\n"));
            builder.Append(CreateBody(data, charCountChecker.Limit));

            return BotClient.SendTextMessageAsync(
                Update.Message.Chat,
                builder.Build().Write(),
                ParseMode.Html,
                cancellationToken: Token
            );
        }

        private IEnumerable<JsonPostResponse>? GetData()
        {
            try
            {
                return FetchAndDeserialize().Result;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Invalid json: {exception}");
                return null;
            }
        }

        private static ITextBlock CreateBody(IEnumerable<JsonPostResponse> result, int limit)
        {
            var charCountChecker = new CharCountChecker(limit);
            return result.Aggregate(
                    new ConditionBlockBuilder("\n", DefaultRegex.DynamicVariableName, charCountChecker),
                    (blockBuilder, response) =>
                    {
                        var postBlock = PostHandler(response);
                        blockBuilder.Append(postBlock);
                        return blockBuilder;
                    }
                )
                .Build();
        }

        private static ITextBlock PostHandler(JsonPostResponse post)
        {
            return TextBlockFactory.MergeTemplates("\n",
                CreateField("User", post.userId.ToString()),
                CreateField("Post", post.id.ToString()),
                CreateField("Title", post.title, new BoldEditor()),
                CreateField("Body", post.body),
                TextBlockFactory.CreateText("")
            );
        }

        private static ITextBlock CreateField(string labelName, string text, ITextEditor? editor = null)
        {
            const string labelVariableName = "NAME";
            const string textVariableName = "TEXT";

            var block = TextBlockFactory.CreateTemplate(
                $"{DefaultRegex.SelectorFactory.Invoke(labelVariableName)}: {DefaultRegex.SelectorFactory.Invoke(textVariableName)}",
                new Dictionary<string, ITextBlock>
                {
                    {labelVariableName, TextBlockFactory.CreateText(labelName)},
                    {textVariableName, TextBlockFactory.CreateText(text)}
                }
            );
            block.Editor = editor;
            return block;
        }

        private async Task<IEnumerable<JsonPostResponse>> FetchAndDeserialize()
        {
            using var response = await _httpClient.GetAsync(Url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (response.Content != null && response.Content.Headers.ContentType.MediaType == "application/json")
            {
                return await Deserialize(response);
            }

            throw new HttpRequestException("HTTP Response was invalid and cannot be deserialized.");
        }

        private async Task<IEnumerable<JsonPostResponse>> Deserialize(HttpResponseMessage response)
        {
            var contentStream = await response.Content.ReadAsStreamAsync();

            using var streamReader = new StreamReader(contentStream);
            using var jsonTextReader = new JsonTextReader(streamReader);
            try
            {
                return _jsonSerializer.Deserialize<IEnumerable<JsonPostResponse>>(jsonTextReader);
            }
            catch (JsonReaderException e)
            {
                throw new JsonSerializationException($"Invalid json: {e}");
            }
        }

        private class BoldEditor : ITextEditor
        {
            public ITextEditor Copy()
            {
                return new BoldEditor();
            }

            public string ToEditing(string text)
            {
                return $"<b>{text}</b>";
            }
        }
    }

    [Serializable]
    public struct JsonPostResponse
    {
        public int userId;
        public int id;
        public string title;
        public string body;
    }
}
