using System;
using System.Collections.Generic;

using TemplateLib.Editor;
using TemplateLib.Writer;

namespace TemplateLib.Block
{
    public class TextBlock : ITextBlock
    {
        public string Variable { get; set; } = "";
        public ITextWriter Writer { get; set; }
        public ITextEditor? Editor { get; set; }

        public TextBlock(ITextWriter writer, ITextEditor? editor)
        {
            Writer = writer ?? throw new ArgumentNullException(nameof(writer));
            Editor = editor;
        }

        public TextBlock(TextBlock block)
        {
            if (block == null) throw new ArgumentNullException(nameof(block));

            Writer = block.Writer.Copy();
            Editor = block.Editor?.Copy();
            Variable = block.Variable;
        }

        public ITextBlock Copy()
        {
            return new TextBlock(this);
        }

        public string Write()
        {
            return Editor != null
                ? Editor.ToEditing(Writer.ToWriting(new Dictionary<string, string>(), Variable))
                : Writer.ToWriting(new Dictionary<string, string>(), Variable);
        }

        public string WriteWithEditor(ITextEditor editor)
        {
            return editor.ToEditing(Writer.ToWriting(new Dictionary<string, string>(), Variable));
        }

        public string WriteWithoutEditor()
        {
            return Writer.ToWriting(new Dictionary<string, string>(), Variable);
        }
    }
}
