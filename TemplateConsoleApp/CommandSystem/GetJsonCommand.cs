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
using TemplateLib.Objects;

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

            try
            {
                var result = FetchAndDeserialize().Result;
                var bodyTextBlock = CreateBody(result, charCountChecker.Limit);
                builder.Put(body, bodyTextBlock);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Invalid json: {exception}");
            }

            return BotClient.SendTextMessageAsync(
                Update.Message.Chat,
                builder.Build().WriteWithEditor(),
                ParseMode.Html,
                cancellationToken: Token
            );
        }

        private static TextBlock CreateBody(IEnumerable<JsonPostResponse> result, int limit)
        {
            var charCountChecker = new CharCountChecker(limit);
            return result.Aggregate(
                    new DynamicCompositeBlockBuilder(charCountChecker, "VAR", "\n"),
                    (blockBuilder, response) =>
                    {
                        var postBlock = PostHandler(response, charCountChecker.Limit);
                        blockBuilder.DynamicPut(postBlock);
                        return blockBuilder;
                    }
                )
                .Build();
        }

        private static TextBlock PostHandler(JsonPostResponse post, int limit)
        {
            const string userId = "USER";
            const string postId = "ID";
            const string title = "TITLE";
            const string body = "BODY";

            var userTextBlock = TextBlockFactory.CreateText(userId, post.userId.ToString());
            var postIdTextBlock = TextBlockFactory.CreateText(postId, post.id.ToString());
            var titleTextBlock = TextBlockFactory.CreateText(title, post.title);
            var bodyTextBlock = TextBlockFactory.CreateText(body, post.body);

            return new[] {userTextBlock, postIdTextBlock, titleTextBlock, bodyTextBlock}
                .Merge("VAR", "\n");
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
