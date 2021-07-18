using TemplateLib.Editor;
using TemplateLib.Writer;

namespace TemplateLib.Block
{
    public interface ITextBlock
    {
        ITextWriter Writer { get; set; }
        ITextEditor? Editor { get; set; }
        ITextBlock Copy();
        string Write();
        string WriteWithEditor(ITextEditor editor);
        string WriteWithoutEditor();
    }
}
