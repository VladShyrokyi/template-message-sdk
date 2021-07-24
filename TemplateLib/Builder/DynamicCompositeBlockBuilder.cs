using TemplateLib.Block;
using TemplateLib.Checker;
using TemplateLib.Exception;
using TemplateLib.Factory;

namespace TemplateLib.Builder
{
    public class DynamicCompositeBlockBuilder : CompositeBlockBuilder
    {
        private readonly string _dynamicVariableName;
        private readonly string _separator;

        private int _dynamicVariableCounter;

        public DynamicCompositeBlockBuilder(string separator,
                                            string dynamicVariableName = DefaultRegex.DynamicVariableName,
                                            IConditionChecker? conditionChecker = null) : base(conditionChecker)
        {
            _dynamicVariableName = dynamicVariableName;
            _dynamicVariableCounter = 0;
            _separator = separator;
        }

        public new DynamicCompositeBlockBuilder Add(string name, string templatePart)
        {
            if (name == null) throw new VariableNameNullException(this);
            if (templatePart == null) throw new TemplateNullException(this);

            return (DynamicCompositeBlockBuilder) base.Add(name, templatePart);
        }

        public new DynamicCompositeBlockBuilder Put(string name, ITextBlock variable)
        {
            if (name == null) throw new VariableNameNullException(this);
            if (variable == null) throw new VariableNullException(this);

            return (DynamicCompositeBlockBuilder) base.Put(name, variable);
        }

        public DynamicCompositeBlockBuilder DynamicPut(ITextBlock block)
        {
            if (block == null) throw new VariableNullException(this);

            var variableName = _dynamicVariableName + "_" + _dynamicVariableCounter;
            var templatePart = _dynamicVariableCounter == 0
                ? DefaultRegex.SelectorFrom(variableName)
                : _separator + DefaultRegex.SelectorFrom(variableName);

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
