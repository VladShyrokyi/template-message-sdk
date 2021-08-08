using System;
using System.Collections.Generic;
using System.Linq;

using TemplateLib.Editor;
using TemplateLib.Exception;
using TemplateLib.Writer;

namespace TemplateLib.Block
{
    public class TemplateBlock : ITextBlockWithVariables, ITextBlockExpendable
    {
        private readonly Dictionary<string, ITextBlock> _variables = new Dictionary<string, ITextBlock>();

        public TemplateBlock(ITextWriter writer, ITextEditor? editor)
        {
            Writer = writer ?? throw new ArgumentNullException(nameof(writer));
            Editor = editor;
        }

        public TemplateBlock(TemplateBlock block)
        {
            if (block == null) throw new ArgumentNullException(nameof(block));

            Writer = block.Writer.Copy();
            Editor = block.Editor?.Copy();
            foreach (var pair in block._variables)
                PutVariable(pair.Key, pair.Value);
        }

        public void Append(string templatePart)
        {
            Writer.Template += templatePart;
        }

        public ITextWriter Writer { get; set; }
        public ITextEditor? Editor { get; set; }

        public ITextBlock Copy()
        {
            return new TemplateBlock(this);
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

        public override string ToString()
        {
            return Write();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TemplateBlock template)) return false;
            if (!Equals(template.Editor, Editor)) return false;
            if (!Equals(template.Writer, Writer)) return false;
            if (!Equals(template._variables.Count, _variables.Count)) return false;
            foreach (var pair in _variables)
            {
                if (!template._variables.TryGetValue(pair.Key, out ITextBlock value)) return false;
                if (!Equals(value, pair.Value)) return false;
            }

            return true;
        }

        protected bool Equals(TemplateBlock other)
        {
            return _variables.Equals(other._variables) && Writer.Equals(other.Writer) && Equals(Editor, other.Editor);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _variables.GetHashCode();
                hashCode = (hashCode * 397) ^ Writer.GetHashCode();
                hashCode = (hashCode * 397) ^ (Editor != null ? Editor.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
