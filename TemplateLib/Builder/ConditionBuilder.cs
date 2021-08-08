using TemplateLib.Block;
using TemplateLib.Checker;
using TemplateLib.Editor;
using TemplateLib.Factory;
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

        public new ITextBlock Build()
        {
            var writer = Writer.Copy();
            var editor = Editor?.Copy();
            var block = new TemplateBlock(writer, editor);

            foreach (var variableName in Variables)
            {
                var templatePart = VariableTemplateParts[variableName];
                var variableValue = VariableValues[variableName];

                var onlyTemplate = TextBlockFactory.CreateText(templatePart);
                if (!ConditionChecker.Check(onlyTemplate)) continue;
                if (!ConditionChecker.Check(variableValue)) continue;

                writer.Template += templatePart;
                ConditionChecker.Update(onlyTemplate);
                block.PutVariable(variableName, variableValue.Copy());
                ConditionChecker.Update(variableValue);
            }

            return block;
        }
    }
}
