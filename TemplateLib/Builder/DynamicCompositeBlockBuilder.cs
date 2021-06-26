using System.Collections.Generic;
using TemplateLib.Factory;
using TemplateLib.Models;

namespace TemplateLib.Builder
{
    public class DynamicCompositeBlockBuilder : CompositeBlockBuilder
    {
        private int _dynamicVariableCounter;
        private readonly string _dynamicVariableName;
        private readonly string _separator;

        public DynamicCompositeBlockBuilder(IConditionChecker conditionChecker,
            string dynamicVariableName, string separator) : base(conditionChecker)
        {
            _dynamicVariableName = dynamicVariableName;
            _dynamicVariableCounter = 0;
            _separator = separator;
        }

        public CompositeBlockBuilder DynamicPut(TextBlock block)
        {
            var variableName = _dynamicVariableName + "_" + _dynamicVariableCounter;
            var templatePart = _dynamicVariableCounter == 0
                ? $"%[{variableName}]%"
                : $"{_separator}%[{variableName}]%";

            var checkedBlock = TextBlockFactory.CreateText(templatePart, new Dictionary<string, TextBlock>
            {
                {variableName, block}
            });
            if (!ConditionChecker.Check(checkedBlock))
                return this;

            Add(variableName, templatePart);
            Put(variableName, block);
            _dynamicVariableCounter++;

            return this;
        }
    }
}
