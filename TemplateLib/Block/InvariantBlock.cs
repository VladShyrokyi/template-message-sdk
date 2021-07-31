using System;
using System.Collections.Generic;

using TemplateLib.Editor;
using TemplateLib.Writer;

namespace TemplateLib.Block
{
    public class InvariantBlock : ITextBlock
    {
        public InvariantBlock(ITextWriter writer, ITextEditor? editor)
        {
            Writer = writer ?? throw new ArgumentNullException(nameof(writer));
            Editor = editor;
        }

        public InvariantBlock(InvariantBlock block)
        {
            if (block == null) throw new ArgumentNullException(nameof(block));

            Writer = block.Writer.Copy();
            Editor = block.Editor?.Copy();
        }

        public ITextWriter Writer { get; set; }

        public ITextEditor? Editor { get; set; }

        public ITextBlock Copy()
        {
            return new InvariantBlock(this);
        }

        public string Write()
        {
            return Editor != null
                ? Editor.ToEditing(Writer.ToWriting(new Dictionary<string, string>()))
                : Writer.ToWriting(new Dictionary<string, string>());
        }

        public string WriteWithEditor(ITextEditor editor)
        {
            return editor.ToEditing(Writer.ToWriting(new Dictionary<string, string>()));
        }

        public string WriteWithoutEditor()
        {
            return Writer.ToWriting(new Dictionary<string, string>());
        }

        public override string ToString()
        {
            return Write();
        }
    }
}
