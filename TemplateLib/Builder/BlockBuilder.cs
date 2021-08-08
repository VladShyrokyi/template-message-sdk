using System.Collections.Generic;

using TemplateLib.Block;
using TemplateLib.Editor;
using TemplateLib.Exception;
using TemplateLib.Writer;

namespace TemplateLib.Builder
{
    public class BlockBuilder : IBlockBuilder
    {
        private readonly string _dynamicVariableName;

        private readonly string _separator;
        protected readonly ITextEditor? Editor;

        protected readonly List<string> Variables = new List<string>();
        protected readonly Dictionary<string, string> VariableTemplateParts = new Dictionary<string, string>();
        protected readonly Dictionary<string, ITextBlock> VariableValues = new Dictionary<string, ITextBlock>();
        protected readonly RegexTextWriter Writer;
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

            var variableName = _dynamicVariableName + "_" + _dynamicVariableCounter;
            Variables.Add(variableName);
            var templatePart = _dynamicVariableCounter == 0
                ? Writer.CreateSelector(variableName)
                : _separator + Writer.CreateSelector(variableName);

            VariableTemplateParts.Add(variableName, templatePart);
            VariableValues.Add(variableName, variable);
            _dynamicVariableCounter++;
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
    }
}
