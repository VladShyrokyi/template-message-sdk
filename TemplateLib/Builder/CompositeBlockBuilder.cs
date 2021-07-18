using System.Collections.Generic;
using System.Linq;
using TemplateLib.Block;
using TemplateLib.Factory;

namespace TemplateLib.Builder
{
    public class CompositeBlockBuilder
    {
        private readonly IConditionChecker? _conditionChecker;
        private readonly Dictionary<string, string> _templateParts = new Dictionary<string, string>();
        private readonly Dictionary<string, ITextBlock> _variables = new Dictionary<string, ITextBlock>();

        public CompositeBlockBuilder(IConditionChecker? conditionChecker = null)
        {
            _conditionChecker = conditionChecker;
        }

        public CompositeBlockBuilder Add(string variableName, string templatePart)
        {
            var checkedBlock = TextBlockFactory.CreateSimpleEmptyWith(templatePart);
            if (IsNotContinueAdd(checkedBlock))
                return this;

            _templateParts.Add(variableName, templatePart);
            UpdateIfCan(checkedBlock);

            return this;
        }

        public CompositeBlockBuilder Put(string variableName, ITextBlock? block)
        {
            if (block == null)
                return this;

            if (IsNotContinueAdd(block))
                return this;

            _variables.Add(variableName, block);
            UpdateIfCan(block);
            return this;
        }

        public TemplateTextBlock Build()
        {
            var template = "";
            _variables.Keys.ToList().ForEach(variableName =>
            {
                if (_templateParts.TryGetValue(variableName, out string templatePart))
                    template += templatePart;
            });
            return TextBlockFactory.CreateTemplateWith(template, _variables);
        }

        protected bool IsNotContinueAdd(ITextBlock checkedBlock)
        {
            return _conditionChecker != null && !_conditionChecker.Check(checkedBlock);
        }

        protected void UpdateIfCan(ITextBlock checkedBlock)
        {
            _conditionChecker?.Update(checkedBlock);
        }
    }
}
