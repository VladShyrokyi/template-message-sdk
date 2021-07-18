namespace TemplateLib.Editor
{
    public interface ITextEditor
    {
        ITextEditor Copy();
        string ToEditing(string text);
    }
}