using System.Collections.Generic;

using TemplateLib.Block;
using TemplateLib.Builder;
using TemplateLib.Writer;

namespace TemplateLib.Factory
{
    public static class TextBlockFactory
    {
        public static SimpleTextBlock CreateSimpleEmptyWith(string templatePart)
        {
            return new SimpleTextBlock(new RegexTextWriter(templatePart, DefaultRegex.Regex), null);
        }

        public static TemplateTextBlock CreateTemplateEmptyWith(string templatePart)
        {
            return new TemplateTextBlock(new RegexTextWriter(templatePart, DefaultRegex.Regex), null);
        }

        public static SimpleTextBlock CreateSimpleWith(string template, Dictionary<string, string> variables)
        {
            var block = new SimpleTextBlock(new RegexTextWriter(template, DefaultRegex.Regex), null);
            foreach (var pair in variables)
            {
                block.PutVariable(pair.Key, pair.Value);
            }

            return block;
        }

        public static SimpleTextBlock CreateSimpleWith(string variable)
        {
            var block = new SimpleTextBlock(new RegexTextWriter(
                                                DefaultRegex.SelectorFrom(DefaultRegex.DynamicVariableName),
                                                DefaultRegex.Regex
                                            ), null);
            block.PutVariable(DefaultRegex.DynamicVariableName, variable);
            return block;
        }

        public static TemplateTextBlock CreateTemplateWith(string template, Dictionary<string, ITextBlock> variables)
        {
            var block = new TemplateTextBlock(new RegexTextWriter(template, DefaultRegex.Regex), null);
            foreach (var pair in variables)
            {
                block.PutVariable(pair.Key, pair.Value);
            }

            return block;
        }

        public static TemplateTextBlock CreateTemplateWith(string separator, params ITextBlock[] variables)
        {
            var builder = new DynamicCompositeBlockBuilder(separator);
            foreach (var block in variables)
            {
                builder.DynamicPut(block);
            }

            return builder.Build();
        }
    }
}
