namespace TemplateLib.Block
{
    public interface ITextBlockWithVariables : ITextBlock
    {
        ITextBlock GetVariable(string name);
        void PutVariable(string name, ITextBlock variable);
    }
}
