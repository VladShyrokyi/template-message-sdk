using TemplateLib.Block;
using TemplateLib.Checker;
using TemplateLib.Editor;
using TemplateLib.Exception;
using TemplateLib.Writer;

namespace TemplateLib.Builder
{
    public class ConditionBlockBuilder : BlockBuilder
    {
        protected readonly IConditionChecker ConditionChecker;

        public ConditionBlockBuilder(string separator, string dynamicVariableName, IConditionChecker conditionChecker,
                                     ITextEditor? editor = null,
                                     RegexTextWriter? writer = null) : base(separator, dynamicVariableName, editor,
            writer)
        {
            ConditionChecker = conditionChecker;
        }

        public new void Append(ITextBlock variable)
        {
            if (variable == null) throw new VariableNullException(this);

            var variableName = CreateVariableName();
            var templatePart = CreateTemplatePart(variableName);

            var writer = Writer.Copy();
            writer.Template = templatePart;
            var onlyTemplate = new InvariantBlock(writer, null);

            if (!ConditionChecker.Check(onlyTemplate)) return;
            if (!ConditionChecker.Check(variable)) return;

            ConditionChecker.Update(onlyTemplate);
            ConditionChecker.Update(variable);

            Variables.Add(variableName);
            VariableTemplateParts.Add(variableName, templatePart);
            VariableValues.Add(variableName, variable);

            UpdateVariableCounter();
        }
    }
}
