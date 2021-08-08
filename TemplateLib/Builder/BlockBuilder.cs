using System.Collections.Generic;

using TemplateLib.Block;
using TemplateLib.Editor;
using TemplateLib.Exception;
using TemplateLib.Writer;

namespace TemplateLib.Builder
{
    public class BlockBuilder : IBlockBuilder
    {
        protected readonly RegexTextWriter Writer;
        protected readonly ITextEditor? Editor;

        protected readonly LinkedList<string> Variables = new LinkedList<string>();
        protected readonly Dictionary<string, string> VariableTemplateParts = new Dictionary<string, string>();
        protected readonly Dictionary<string, ITextBlock> VariableValues = new Dictionary<string, ITextBlock>();

        private readonly string _separator;
        private readonly string _dynamicVariableName;
        private int _dynamicVariableCounter = 0;

        public BlockBuilder(string separator, string dynamicVariableName, ITextEditor? editor = null,
                            RegexTextWriter? writer = null)
        {
            Writer = writer ?? new RegexTextWriter("", DefaultRegex.Regex, DefaultRegex.SelectorFactory);
            Editor = editor;
            _separator = separator;
            _dynamicVariableName = dynamicVariableName;
        }

        public void Append(ITextBlock variable)
        {
            if (variable == null) throw new VariableNullException(this);

            var variableName = CreateVariableName();
            var templatePart = CreateTemplatePart(variableName);

            Variables.AddLast(variableName);
            VariableTemplateParts.Add(variableName, templatePart);
            VariableValues.Add(variableName, variable);

            UpdateVariableCounter();
        }

        public ITextBlock Build()
        {
            var writer = Writer.Copy();
            var editor = Editor?.Copy();
            var block = new TemplateBlock(writer, editor);

            foreach (var variableName in Variables)
            {
                writer.Template += VariableTemplateParts[variableName];
                var variableValue = VariableValues[variableName];
                block.PutVariable(variableName, variableValue.Copy());
            }

            return block;
        }

        protected string CreateVariableName()
        {
            return _dynamicVariableName + "_" + _dynamicVariableCounter;
        }

        protected string CreateTemplatePart(string variableName)
        {
            return _dynamicVariableCounter == 0
                ? Writer.CreateSelector(variableName)
                : _separator + Writer.CreateSelector(variableName);
        }

        protected void UpdateVariableCounter()
        {
            _dynamicVariableCounter++;
        }
    }
}
