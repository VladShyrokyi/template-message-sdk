namespace TemplateLib.Block
{
    public interface ITextBlockExpendable : ITextBlock
    {
        void Append(string templatePart);
    }
}
