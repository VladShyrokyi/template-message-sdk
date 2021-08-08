using System.Collections.Generic;

using TemplateLib.Block;
using TemplateLib.Checker;
using TemplateLib.Factory;

namespace TemplateLib.Builder
{
    public class ConditionBlockBuilder : IBlockBuilder
    {
        protected readonly IBlockBuilder BlockBuilder;
        protected readonly IConditionChecker ConditionChecker;

        public ConditionBlockBuilder(IBlockBuilder blockBuilder, IConditionChecker conditionChecker)
        {
            BlockBuilder = blockBuilder;
            ConditionChecker = conditionChecker;
        }

        public List<string> CopyVariables() => BlockBuilder.CopyVariables();
        public Dictionary<string, string> CopyTemplateParts() => BlockBuilder.CopyTemplateParts();
        public Dictionary<string, ITextBlock> CopyVariablesValue() => BlockBuilder.CopyVariablesValue();

        public void Append(ITextBlock variable)
        {
            BlockBuilder.Append(variable);
        }

        public ITextBlock Build()
        {
            var build = BlockBuilder.Build();
            var writer = build.Writer.Copy();
            writer.Template = "";
            var editor = build.Editor?.Copy();
            var block = new TemplateBlock(writer, editor);

            var templateParts = CopyTemplateParts();
            var variablesValue = CopyVariablesValue();

            foreach (var variableName in CopyVariables())
            {
                var templatePart = templateParts[variableName];
                var onlyTemplate = TextBlockFactory.CreateOnlyTemplate(templatePart);
                var variableValue = variablesValue[variableName];

                if (!ConditionChecker.Check(onlyTemplate)) continue;
                if (!ConditionChecker.Check(variableValue)) continue;

                writer.Template += templatePart;
                ConditionChecker.Update(onlyTemplate);

                block.PutVariable(variableName, variableValue);
                ConditionChecker.Update(variableValue);
            }

            return block;
        }
    }
}
