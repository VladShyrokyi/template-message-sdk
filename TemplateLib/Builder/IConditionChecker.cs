using TemplateLib.Objects;

namespace TemplateLib.Builder
{
    public interface IConditionChecker
    {
        bool Check(TextBlock block);

        void Update(TextBlock block);
    }
}
