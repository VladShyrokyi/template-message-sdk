﻿using System.Collections.Generic;

using TemplateLib.Block;
using TemplateLib.Editor;
using TemplateLib.Exception;
using TemplateLib.Writer;

namespace TemplateLib.Builder
{
    public class TemplateBlockBuilder
    {
        protected readonly ITextEditor? Editor;
        protected readonly LinkedList<string> TemplateParts = new LinkedList<string>();
        protected readonly Dictionary<string, ITextBlock> Variables = new Dictionary<string, ITextBlock>();

        protected readonly ITextWriter Writer;

        public TemplateBlockBuilder(ITextWriter writer, ITextEditor? editor)
        {
            Writer = writer;
            Editor = editor;
        }

        protected string Template => string.Join("", TemplateParts);

        public void Append(string templatePart)
        {
            if (templatePart == null) throw new TemplateNullException(this);

            TemplateParts.AddLast(templatePart);
        }

        public void PutVariable(string name, ITextBlock variable)
        {
            if (name == null) throw new VariableNameNullException(this);
            if (variable == null) throw new VariableNullException(this);

            Variables.Add(name, variable);
        }

        public void PutVariables(Dictionary<string, ITextBlock> variables)
        {
            if (variables == null) throw new VariableNullException(this);

            foreach (var pair in variables)
                Variables.Add(pair.Key, pair.Value);
        }

        public ITextBlock Build()
        {
            Writer.Template += Template;
            var block = new TemplateBlock(Writer.Copy(), Editor?.Copy());
            foreach (var pair in Variables)
                block.PutVariable(pair.Key, pair.Value);
            return block;
        }

        public override string ToString()
        {
            return Build().Write();
        }
    }
}
