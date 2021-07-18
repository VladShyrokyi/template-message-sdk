using TemplateLib.Block;
using TemplateLib.Builder;

namespace TemplateConsoleApp.CommandSystem
{
    public class CharCountChecker : IConditionChecker
    {
        public CharCountChecker(int maxCharCount)
        {
            Limit = maxCharCount;
        }

        public int Limit { get; private set; } = 0;

        public bool Check(ITextBlock block)
        {
            return Limit - block.Write().Length >= 0;
        }

        public void Update(ITextBlock block)
        {
            Limit -= block.Write().Length;
        }
    }
}