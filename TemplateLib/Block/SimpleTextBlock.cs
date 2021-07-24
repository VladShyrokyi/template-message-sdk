using System;
using System.Collections.Generic;

using TemplateLib.Editor;
using TemplateLib.Exception;
using TemplateLib.Writer;

namespace TemplateLib.Block
{
    public class SimpleTextBlock : ITextBlock
    {
        private readonly Dictionary<string, string> _variables = new Dictionary<string, string>();

        public SimpleTextBlock(ITextWriter writer, ITextEditor? editor)
        {
            Writer = writer ?? throw new ArgumentNullException(nameof(writer));
            Editor = editor;
        }

        public SimpleTextBlock(SimpleTextBlock block)
        {
            if (block == null) throw new ArgumentNullException(nameof(block));

            Writer = block.Writer.Copy();
            Editor = block.Editor?.Copy();
            foreach (var pair in block._variables)
                PutVariable(pair.Key, pair.Value);
        }

        public ITextWriter Writer { get; set; }

        public ITextEditor? Editor { get; set; }

        public ITextBlock Copy()
        {
            return new SimpleTextBlock(this);
        }

        public string Write()
        {
            return Editor != null
                ? Editor.ToEditing(Writer.ToWriting(new Dictionary<string, string>(_variables), ""))
                : Writer.ToWriting(new Dictionary<string, string>(_variables), "");
        }

        public string WriteWithEditor(ITextEditor editor)
        {
            return editor.ToEditing(Writer.ToWriting(new Dictionary<string, string>(_variables), ""));
        }

        public string WriteWithoutEditor()
        {
            return Writer.ToWriting(new Dictionary<string, string>(_variables), "");
        }

        public string GetVariable(string name)
        {
            if (name == null) throw new VariableNameNullException(this);

            return _variables[name];
        }

        public void PutVariable(string name, string variable)
        {
            if (name == null) throw new VariableNameNullException(this);
            if (variable == null) throw new VariableNullException(this);

            _variables.Add(name, variable);
        }
    }
}
