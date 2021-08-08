using System.Collections.Generic;

using TemplateLib.Block;

namespace TemplateLib.Builder
{
    public interface IBlockBuilder
    {
        public List<string> CopyVariables();
        public Dictionary<string, string> CopyTemplateParts();
        public Dictionary<string, ITextBlock> CopyVariablesValue();
        public void Append(ITextBlock variable);
        public ITextBlock Build();
    }
}
