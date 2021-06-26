using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TemplateLib.Builder;
using TemplateLib.Factory;
using TemplateLib.Models;

namespace TemplateConsoleApp.CommandSystem
{
    internal class GetJsonCommand : TelegramBotCommand
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly JsonSerializer _jsonSerializer = JsonSerializer.CreateDefault();
        private const string Url = "https://jsonplaceholder.typicode.com/posts";
        private const int MaxCharCount = 4096;

        public GetJsonCommand(string name, CommandContext context) : base(name, context)
        {
        }

        public override Task<Message> Execute()
        {
            const string title = "TITLE";
            const string body = "BODY";

            var charCountChecker = new CharCountChecker(MaxCharCount);
            var builder = new CompositeBlockBuilder(charCountChecker);

            builder.Add(title, $"%[{title}]%");
            builder.Add(body, $"\n%[{body}]%");

            builder.Put(title, new TextBlock($"Response from %[{title}]%")
                .PutVariable(title, Url)
                .SetTemplateEditor(text => $"<b>{text}</b>")
            );

            var data = GetData();
            if (data == null)
            {
                return BotClient.SendTextMessageAsync(
                    Update.Message.Chat,
                    builder.Build().WriteWithEditor(),
                    ParseMode.Html,
                    cancellationToken: Token
                );
            }

            var bodyTextBlock = CreateBody(data, charCountChecker.Limit);
            builder.Put(body, bodyTextBlock);

            return BotClient.SendTextMessageAsync(
                Update.Message.Chat,
                builder.Build().WriteWithEditor(),
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

        private static TextBlock CreateBody(IEnumerable<JsonPostResponse> result, int limit)
        {
            var charCountChecker = new CharCountChecker(limit);
            return result.Aggregate(
                    new DynamicCompositeBlockBuilder(charCountChecker, "VAR", "\n"),
                    (blockBuilder, response) =>
                    {
                        var postBlock = PostHandler(response);
                        blockBuilder.DynamicPut(postBlock);
                        return blockBuilder;
                    }
                )
                .Build();
        }

        private static TextBlock PostHandler(JsonPostResponse post)
        {
            var user = CreateField("User", post.userId.ToString());
            var postId = CreateField("\nPost", post.id.ToString());
            var title = CreateField("\nTitle", post.title)
                .SetTemplateEditor(str => $"<b>{str}</b>");
            var body = CreateField("\nBody: ", post.body);
            var append = TextBlockFactory.CreateText("VAR", "\n");

            return TextBlockFactory.MergeText("VAR", "", user, postId, title, body, append);
        }

        private static TextBlock CreateField(string labelName, string text)
        {
            const string labelVariableName = "NAME";
            const string textVariableName = "TEXT";
            var labelTemplate = $"%[{labelVariableName}]%: %[{textVariableName}]%";

            var textBlock = new TextBlock(labelTemplate).PutVariable(labelVariableName, labelName)
                .PutVariable(textVariableName, text);
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
