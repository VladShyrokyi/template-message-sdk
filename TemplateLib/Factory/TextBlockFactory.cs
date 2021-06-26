using System.Collections.Generic;
using System.Linq;
using TemplateLib.Models;

namespace TemplateLib.Factory
{
    public static class TextBlockFactory
    {
        public static TextBlock CreateText(string variableName, string variableValue)
        {
            return new TextBlock($"%[{variableName}]%")
                .PutVariable(variableName, variableValue);
        }

        public static TextBlock CreateText(string template, Dictionary<string, TextBlock> variableValue)
        {
            return new TextBlock(template).PutVariables(variableValue);
        }

        public static TextBlock MergeText(string dynamicVariableName, string separator, params TextBlock[] textBlocks)
        {
            return textBlocks.Aggregate(
                (block: new TextBlock(), template: "", counter: 0, lastTextBlock: new TextBlock()),
                (container, textBlock) =>
                {
                    if (!Equals(container.lastTextBlock, new TextBlock()))
                    {
                        container.template += separator;
                    }

                    var variableName = dynamicVariableName + "_" + container.counter;

                    container.template += $"%[{variableName}]%";
                    container.block.SetTemplate(container.template);
                    container.block.PutVariable(variableName, textBlock.CopyTemplate());
                    container.lastTextBlock = textBlock;
                    container.counter++;
                    return container;
                })
                .block;
        }
    }
}
