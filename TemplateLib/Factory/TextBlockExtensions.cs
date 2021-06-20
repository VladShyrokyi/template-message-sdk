using System.Collections.Generic;
using System.Linq;
using TemplateLib.Objects;

namespace TemplateLib.Factory
{
    public static class TextBlockExtensions
    {
        public static TextBlock Merge(this IEnumerable<TextBlock> blocks, string dynamicVariableName, string separator)
        {
            return TextBlockFactory.CreateText(dynamicVariableName, separator, blocks.ToArray());
        }
    }
}
