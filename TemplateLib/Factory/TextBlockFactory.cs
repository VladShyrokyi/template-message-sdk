using System.Linq;
using TemplateLib.Objects;

namespace TemplateLib.Factory
{
    public static class TextBlockFactory
    {
        public static TextBlock CreateText(string variableName, string variableValue)
        {
            return new TextBlock($"%[{variableName}]%")
                .PutVariable(variableName, variableValue);
        }

        public static TextBlock CreateText(string dynamicVariableName, string separator, params TextBlock[] textBlocks)
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
