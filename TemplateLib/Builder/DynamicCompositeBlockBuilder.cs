﻿using TemplateLib.Block;
using TemplateLib.Factory;

namespace TemplateLib.Builder
{
    public class DynamicCompositeBlockBuilder : CompositeBlockBuilder
    {
        private readonly string _dynamicVariableName;
        private readonly string _separator;
        private int _dynamicVariableCounter;

        public DynamicCompositeBlockBuilder(string dynamicVariableName, string separator,
            IConditionChecker? conditionChecker = null) : base(conditionChecker)
        {
            _dynamicVariableName = dynamicVariableName;
            _dynamicVariableCounter = 0;
            _separator = separator;
        }

        public new DynamicCompositeBlockBuilder Add(string name, string templatePart)
        {
            return (DynamicCompositeBlockBuilder) base.Add(name, templatePart);
        }

        public new DynamicCompositeBlockBuilder Put(string name, ITextBlock variable)
        {
            return (DynamicCompositeBlockBuilder) base.Put(name, variable);
        }

        public DynamicCompositeBlockBuilder DynamicPut(ITextBlock block)
        {
            var variableName = _dynamicVariableName + "_" + _dynamicVariableCounter;
            var templatePart = _dynamicVariableCounter == 0
                ? $"%[{variableName}]%"
                : $"{_separator}%[{variableName}]%";

            var checkedBlock = TextBlockFactory.CreateTemplateEmptyWith(templatePart);
            checkedBlock.PutVariable(variableName, block);
            if (IsNotContinueAdd(checkedBlock))
                return this;

            Add(variableName, templatePart).Put(variableName, block);
            _dynamicVariableCounter++;

            return this;
        }
    }
}
