using TemplateLib.Block;

namespace TemplateLib.Checker
{
    public class CharCountChecker : IConditionChecker
    {
        public CharCountChecker(int limit)
        {
            Limit = limit;
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
