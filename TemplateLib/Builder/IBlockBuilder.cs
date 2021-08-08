using TemplateLib.Block;

namespace TemplateLib.Builder
{
    public interface IBlockBuilder
    {
        public void Append(ITextBlock variable);
        public ITextBlock Build();
    }
}
