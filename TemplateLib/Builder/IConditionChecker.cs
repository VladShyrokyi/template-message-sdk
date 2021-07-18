using TemplateLib.Block;

namespace TemplateLib.Builder
{
    public interface IConditionChecker
    {
        bool Check(ITextBlock block);

        void Update(ITextBlock block);
    }
}