using System.Collections.Generic;

using TemplateLib.Block;
using TemplateLib.Builder;
using TemplateLib.Checker;
using TemplateLib.Exception;
using TemplateLib.Writer;

namespace TemplateLib.Factory
{
    public static class TextBlockFactory
    {
        public static ITextBlock CreateOnlyTemplate(string template)
        {
            if (template == null) throw new TemplateNullException(typeof(TextBlockFactory));

            return new InvariantBlock(
                new RegexTextWriter(template, DefaultRegex.Regex, DefaultRegex.SelectorFactory), null);
        }

        public static ITextBlock CreateText(string variable)
        {
            if (variable == null) throw new VariableNullException(typeof(TextBlockFactory));

            return new TextBlock(new RegexTextWriter(
                DefaultRegex.SelectorFactory.Invoke(DefaultRegex.DynamicVariableName),
                DefaultRegex.Regex,
                DefaultRegex.SelectorFactory
            ), null, variable);
        }

        public static ITextBlock CreateTemplate(string template, Dictionary<string, ITextBlock> variables)
        {
            if (template == null) throw new TemplateNullException(typeof(TextBlockFactory));
            if (variables == null) throw new VariableNullException(typeof(TextBlockFactory));

            var block = new TemplateBlock(
                new RegexTextWriter(template, DefaultRegex.Regex, DefaultRegex.SelectorFactory), null);
            foreach (var pair in variables)
                block.PutVariable(pair.Key, pair.Value);

            return block;
        }

        public static TemplateBlockDynamicBuilder DynamicBuilder(string separator = "", string template = "",
                                                                 string dynamicVariableName =
                                                                     DefaultRegex.DynamicVariableName)
        {
            return new TemplateBlockDynamicBuilder(new RegexTextWriter(
                template,
                DefaultRegex.Regex,
                DefaultRegex.SelectorFactory
            ), null, separator, dynamicVariableName);
        }

        public static TemplateBlockConditionDynamicBuilder ConditionDynamicBuilder(
            IConditionChecker checker, string separator = "", string template = "",
            string dynamicVariableName = DefaultRegex.DynamicVariableName)
        {
            return new TemplateBlockConditionDynamicBuilder(new RegexTextWriter(
                template,
                DefaultRegex.Regex,
                DefaultRegex.SelectorFactory
            ), null, checker, separator, dynamicVariableName);
        }
    }
}
