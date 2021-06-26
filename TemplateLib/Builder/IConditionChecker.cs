using TemplateLib.Models;

namespace TemplateLib.Builder
{
    public interface IConditionChecker
    {
        bool Check(TextBlock block);

        void Update(TextBlock block);
    }
}
