using System;
using TemplateLib.Builder;
using TemplateLib.Models;

namespace TemplateConsoleApp.CommandSystem
{
    public class CharCountChecker : IConditionChecker
    {
        public int Limit { get; private set; } = 0;

        public CharCountChecker(int maxCharCount)
        {
            Limit = maxCharCount;
        }

        public bool Check(TextBlock block)
        {
            return Limit - block.GetCharCountWithEditor() >= 0;
        }

        public void Update(TextBlock block)
        {
            Limit -= block.GetCharCountWithEditor();
        }
    }
}
