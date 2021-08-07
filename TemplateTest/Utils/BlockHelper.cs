using System.Collections.Generic;

using TemplateLib;
using TemplateLib.Block;
using TemplateLib.Editor;
using TemplateLib.Writer;

namespace TemplateTest.Utils
{
    public static class BlockHelper
    {
        public static TemplateBlock CreateTemplateBlock(string template,
                                                        Dictionary<string, ITextBlock> variables = null,
                                                        ITextEditor editor = null)
        {
            var writer = new RegexTextWriter(template, DefaultRegex.Regex, DefaultRegex.SelectorFactory);
            var block = new TemplateBlock(writer, editor);

            if (variables == null)
                return block;

            foreach (var pair in variables)
                block.PutVariable(pair.Key, pair.Value);

            return block;
        }
    }
}
