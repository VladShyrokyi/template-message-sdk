using System;
using System.Collections.Generic;

using TemplateLib.Block;
using TemplateLib.Builder;
using TemplateLib.Exception;
using TemplateLib.Writer;

namespace TemplateLib.Factory
{
    public static class TextBlockFactory
    {
        public static SimpleTextBlock CreateSimpleEmptyWith(string template)
        {
            if (template == null) throw new TemplateNullException(typeof(TextBlockFactory));

            return new SimpleTextBlock(new RegexTextWriter(template, DefaultRegex.Regex), null);
        }

        public static TemplateTextBlock CreateTemplateEmptyWith(string template)
        {
            if (template == null) throw new TemplateNullException(typeof(TextBlockFactory));

            return new TemplateTextBlock(new RegexTextWriter(template, DefaultRegex.Regex), null);
        }

        public static SimpleTextBlock CreateSimpleWith(string template, Dictionary<string, string> variables)
        {
            if (template == null) throw new TemplateNullException(typeof(TextBlockFactory));
            if (variables == null) throw new VariableNullException(typeof(TextBlockFactory));

            var block = new SimpleTextBlock(new RegexTextWriter(template, DefaultRegex.Regex), null);
            foreach (var pair in variables)
                block.PutVariable(pair.Key, pair.Value);

            return block;
        }

        public static SimpleTextBlock CreateSimpleWith(string variable)
        {
            if (variable == null) throw new VariableNullException(typeof(TextBlockFactory));

            var block = new SimpleTextBlock(new RegexTextWriter(
                                                DefaultRegex.SelectorFrom(DefaultRegex.DynamicVariableName),
                                                DefaultRegex.Regex
                                            ), null);
            block.PutVariable(DefaultRegex.DynamicVariableName, variable);
            return block;
        }

        public static TemplateTextBlock CreateTemplateWith(string template, Dictionary<string, ITextBlock> variables)
        {
            if (template == null) throw new TemplateNullException(typeof(TextBlockFactory));
            if (variables == null) throw new VariableNullException(typeof(TextBlockFactory));

            var block = new TemplateTextBlock(new RegexTextWriter(template, DefaultRegex.Regex), null);
            foreach (var pair in variables)
                block.PutVariable(pair.Key, pair.Value);

            return block;
        }

        public static TemplateTextBlock CreateTemplateWith(string separator, params ITextBlock[] variables)
        {
            if (separator == null) throw new ArgumentNullException(nameof(separator));
            if (variables == null) throw new TemplateNullException(typeof(TextBlockFactory));

            var builder = new DynamicCompositeBlockBuilder(separator);
            foreach (var block in variables)
                builder.DynamicPut(block);

            return builder.Build();
        }
    }
}
