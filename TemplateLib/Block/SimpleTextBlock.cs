using System;
using System.Collections.Generic;
using TemplateLib.Editor;
using TemplateLib.Writer;

namespace TemplateLib.Block
{
    public class SimpleTextBlock : ITextBlock
    {
        private readonly Dictionary<string, string> _variables = new Dictionary<string, string>();

        public SimpleTextBlock(ITextWriter writer, ITextEditor? editor)
        {
            Writer = writer;
            Editor = editor;
        }

        public SimpleTextBlock(SimpleTextBlock block)
        {
            Writer = block.Writer.Copy();
            Editor = block.Editor?.Copy();
            foreach (var pair in block._variables)
            {
                PutVariable(pair.Key, pair.Value);
            }
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
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return _variables[name];
        }

        public void PutVariable(string name, string variable)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (variable == null)
            {
                throw new ArgumentNullException(nameof(variable));
            }

            _variables.Add(name, variable);
        }
    }
}
