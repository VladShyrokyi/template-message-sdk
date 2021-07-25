using System;

using TemplateLib.Block;
using TemplateLib.Checker;
using TemplateLib.Editor;
using TemplateLib.Exception;
using TemplateLib.Writer;

namespace TemplateLib.Builder
{
    public class TemplateBlockConditionDynamicBuilder : TemplateBlockConditionBuilder
    {
        protected new readonly RegexTextWriter Writer;

        private readonly string _dynamicVariableName;
        private readonly string _separator;

        private int _dynamicVariableCounter;

        public TemplateBlockConditionDynamicBuilder(RegexTextWriter writer, ITextEditor? editor,
                                                    IConditionChecker? conditionChecker, string separator,
                                                    string dynamicVariableName) : base(writer, editor, conditionChecker)
        {
            Writer = writer;
            _separator = separator ?? throw new ArgumentNullException(nameof(separator));
            _dynamicVariableName = dynamicVariableName;
            _dynamicVariableCounter = 0;
        }

        public void Append(ITextBlock variable)
        {
            if (variable == null) throw new VariableNullException(this);

            var variableName = _dynamicVariableName + "_" + _dynamicVariableCounter;
            var templatePart = _dynamicVariableCounter == 0
                ? Writer.CreateSelector(variableName)
                : _separator + Writer.CreateSelector(variableName);
            if (!TryAppend(templatePart)) return;

            PutVariable(variableName, variable);
            _dynamicVariableCounter++;
        }
    }
}
