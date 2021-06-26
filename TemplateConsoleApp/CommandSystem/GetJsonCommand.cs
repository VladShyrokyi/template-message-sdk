using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
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

            var titleTemplate = $"%[{title}]%";
            var bodyTemplate = $"\n%[{body}]%";

            var template = "";
            var variables = new Dictionary<string, TextBlock>();
            var charCountLimit = MaxCharCount;

            var headTextBlock = new TextBlock($"Response from %[{title}]%")
                .PutVariable(title, Url)
                .SetTemplateEditor(text => $"<b>{text}</b>");
            if (charCountLimit - headTextBlock.GetCharCountWithEditor() >= 0)
            {
                template += titleTemplate;
                variables.Add(title, headTextBlock);
                charCountLimit -= headTextBlock.GetCharCountWithEditor();
            }

            try
            {
                var result = FetchAndDeserialize().Result;
                var bodyTextBlock = Calculate(result, charCountLimit, "\n")
                    .Merge("VAR", "\n");
                if (charCountLimit - bodyTextBlock.GetCharCountWithEditor() >= 0)
                {
                    template += bodyTemplate;
                    variables.Add(body, bodyTextBlock);
                    charCountLimit -= bodyTextBlock.GetCharCountWithEditor();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Invalid json: {exception}");
            }

            var block = TextBlockFactory.CreateText(template, variables);

            var str = block.WriteWithEditor();
            if (str.Length >= MaxCharCount)
            {
                str = str.Substring(0, MaxCharCount);
            }

            return BotClient.SendTextMessageAsync(
                Update.Message.Chat,
                str,
                ParseMode.Html,
                cancellationToken: Token
            );
        }

        private static IEnumerable<TextBlock> Calculate(IEnumerable<JsonPostResponse> posts, int maxCharCount, string separator)
        {
            var blocks = new List<TextBlock>();
            return posts.Aggregate((blocks, maxCharCount), (container, response) =>
            {
                var block = PostHandler(response);
                var charCount = block.GetCharCountWithEditor() + separator.Length;
                if (container.maxCharCount - charCount <= 0)
                    return container;

                container.blocks.Add(block);
                container.maxCharCount -= charCount;

                return container;
            }).blocks;
        }

        private static TextBlock PostHandler(JsonPostResponse post)
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
