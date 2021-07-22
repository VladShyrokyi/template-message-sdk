using TemplateLib.Block;

namespace TemplateLib.Checker
{
    public interface IConditionChecker
    {
        bool Check(ITextBlock block);

        void Update(ITextBlock block);
    }
}
