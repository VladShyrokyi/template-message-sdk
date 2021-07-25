using System;

using TemplateLib.Block;
using TemplateLib.Checker;
using TemplateLib.Exception;
using TemplateLib.Factory;

namespace TemplateLib.Builder
{
    public class TemplateBlockConditionDynamicBuilder : TemplateBlockConditionBuilder
    {
        private readonly string _dynamicVariableName;
        private readonly string _separator;

        private int _dynamicVariableCounter;

        public TemplateBlockConditionDynamicBuilder(string separator,
                                            string dynamicVariableName = DefaultRegex.DynamicVariableName,
                                            IConditionChecker? conditionChecker = null) : base(conditionChecker)
        {
            _separator = separator ?? throw new ArgumentNullException(nameof(separator));
            _dynamicVariableName = dynamicVariableName;
            _dynamicVariableCounter = 0;
        }

        public new TemplateBlockConditionDynamicBuilder Add(string name, string templatePart)
        {
            if (name == null) throw new VariableNameNullException(this);
            if (templatePart == null) throw new TemplateNullException(this);

            return (TemplateBlockConditionDynamicBuilder) base.Add(name, templatePart);
        }

        public new TemplateBlockConditionDynamicBuilder Put(string name, ITextBlock variable)
        {
            if (name == null) throw new VariableNameNullException(this);
            if (variable == null) throw new VariableNullException(this);

            return (TemplateBlockConditionDynamicBuilder) base.Put(name, variable);
        }

        public TemplateBlockConditionDynamicBuilder DynamicPut(ITextBlock variable)
        {
            if (variable == null) throw new VariableNullException(this);

            var variableName = _dynamicVariableName + "_" + _dynamicVariableCounter;
            var templatePart = _dynamicVariableCounter == 0
                ? DefaultRegex.SelectorFrom(variableName)
                : _separator + DefaultRegex.SelectorFrom(variableName);

            var checkedBlock = TextBlockFactory.CreateTemplateEmptyWith(templatePart);
            checkedBlock.PutVariable(variableName, variable);
            if (IsNotContinueAdd(checkedBlock))
                return this;

            Add(variableName, templatePart).Put(variableName, variable);
            _dynamicVariableCounter++;

            return this;
        }
    }
}
