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
            var builder = new CompositeBlockBuilder(charCountChecker);

            builder.Add(title, DefaultRegex.SelectorFrom(title));
            builder.Add(body, "\n" + DefaultRegex.SelectorFrom(body));

            var titleBlock =
                TextBlockFactory.CreateSimpleEmptyWith($"Response from " + DefaultRegex.SelectorFrom(title));
            titleBlock.PutVariable(title, Url);
            titleBlock.Editor = new BoldEditor();
            builder.Put(title, titleBlock);

            var data = GetData();
            if (data == null)
            {
                return BotClient.SendTextMessageAsync(
                    Update.Message.Chat,
                    builder.Build().Write(),
                    ParseMode.Html,
                    cancellationToken: Token
                );
            }

            var bodyTextBlock = CreateBody(data, charCountChecker.Limit);
            builder.Put(body, bodyTextBlock);

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
                    new DynamicCompositeBlockBuilder("\n", "POST", charCountChecker),
                    (blockBuilder, response) =>
                    {
                        var postBlock = PostHandler(response);
                        blockBuilder.DynamicPut(postBlock);
                        return blockBuilder;
                    }
                )
                .Build();
        }

        private static ITextBlock PostHandler(JsonPostResponse post)
        {
            var title = CreateField("Title", post.title);
            title.Editor = new BoldEditor();

            return new SimpleDynamicCompositeBlockBuilder("\n")
                .DynamicPut(CreateField("User", post.userId.ToString()))
                .DynamicPut(CreateField("Post", post.id.ToString()))
                .DynamicPut(title)
                .DynamicPut(CreateField("Body", post.body))
                .DynamicPut("")
                .Build();
        }

        private static SimpleTextBlock CreateField(string labelName, string text)
        {
            const string labelVariableName = "NAME";
            const string textVariableName = "TEXT";

            var textBlock = TextBlockFactory.CreateSimpleEmptyWith(
                $"{DefaultRegex.SelectorFrom(labelVariableName)}: {DefaultRegex.SelectorFrom(textVariableName)}"
            );
            textBlock.PutVariable(labelVariableName, labelName);
            textBlock.PutVariable(textVariableName, text);
            return textBlock;
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
