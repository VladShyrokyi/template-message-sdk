using System;
using System.Collections.Generic;
using System.Linq;

using TemplateLib.Editor;
using TemplateLib.Exception;
using TemplateLib.Writer;

namespace TemplateLib.Block
{
    public class TemplateTextBlock : ITextBlock
    {
        private readonly Dictionary<string, ITextBlock> _variables = new Dictionary<string, ITextBlock>();

        public TemplateTextBlock(ITextWriter writer, ITextEditor? editor)
        {
            Writer = writer ?? throw new ArgumentNullException(nameof(writer));
            Editor = editor;
        }

        public TemplateTextBlock(TemplateTextBlock block)
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
            return new TemplateTextBlock(this);
        }

        public string Write()
        {
            var variables = _variables
                .ToDictionary(pair => pair.Key, pair => pair.Value.Write());

            return Editor != null
                ? Editor.ToEditing(Writer.ToWriting(variables))
                : Writer.ToWriting(variables);
        }

        public string WriteWithEditor(ITextEditor editor)
        {
            var variables = _variables
                .ToDictionary(pair => pair.Key, pair => pair.Value.WriteWithEditor(editor));

            return editor.ToEditing(Writer.ToWriting(variables));
        }

        public string WriteWithoutEditor()
        {
            var variables = _variables
                .ToDictionary(pair => pair.Key, pair => pair.Value.WriteWithoutEditor());

            return Writer.ToWriting(variables);
        }

        public ITextBlock GetVariable(string name)
        {
            if (name == null) throw new VariableNameNullException(this);

            return _variables[name];
        }

        public void PutVariable(string name, ITextBlock variable)
        {
            if (name == null) throw new VariableNameNullException(this);
            if (variable == null) throw new VariableNullException(this);

            _variables.Add(name, variable);
        }
    }
}
