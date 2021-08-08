using System.Collections.Generic;
using System.Linq;

using TemplateLib.Block;
using TemplateLib.Editor;
using TemplateLib.Exception;
using TemplateLib.Writer;

namespace TemplateLib.Builder
{
    public class BlockBuilder : IBlockBuilder
    {
        private readonly string _dynamicVariableName;
        private readonly ITextEditor? _editor;

        private readonly string _separator;

        private readonly List<string> _variables = new List<string>();
        private readonly Dictionary<string, string> _variableTemplateParts = new Dictionary<string, string>();
        private readonly Dictionary<string, ITextBlock> _variableValues = new Dictionary<string, ITextBlock>();
        private readonly RegexTextWriter _writer;

        private int _dynamicVariableCounter = 0;

        public BlockBuilder(string separator, string dynamicVariableName, ITextEditor? editor = null,
                            RegexTextWriter? writer = null)
        {
            _writer = writer ?? new RegexTextWriter("", DefaultRegex.Regex, DefaultRegex.SelectorFactory);
            _editor = editor;
            _separator = separator;
            _dynamicVariableName = dynamicVariableName;
        }

        public List<string> CopyVariables() => new List<string>(_variables);

        public Dictionary<string, string> CopyTemplateParts() => _variableTemplateParts
            .ToDictionary(pair => pair.Key, pair => pair.Value);

        public Dictionary<string, ITextBlock> CopyVariablesValue() => _variableValues
            .ToDictionary(pair => pair.Key, pair => pair.Value.Copy());

        public void Append(ITextBlock variable)
        {
            if (variable == null) throw new VariableNullException(this);

            var variableName = _dynamicVariableName + "_" + _dynamicVariableCounter;
            _variables.Add(variableName);
            var templatePart = _dynamicVariableCounter == 0
                ? _writer.CreateSelector(variableName)
                : _separator + _writer.CreateSelector(variableName);

            _variableTemplateParts.Add(variableName, templatePart);
            _variableValues.Add(variableName, variable);
            _dynamicVariableCounter++;
        }

        public ITextBlock Build()
        {
            var writer = _writer.Copy();
            var editor = _editor?.Copy();
            var block = new TemplateBlock(writer, editor);

            foreach (var variableName in _variables)
            {
                writer.Template += _variableTemplateParts[variableName];
                var variableValue = _variableValues[variableName];
                block.PutVariable(variableName, variableValue);
            }

            return block;
        }
    }
}
