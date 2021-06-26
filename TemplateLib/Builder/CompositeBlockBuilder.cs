using System.Collections.Generic;
using System.Linq;
using TemplateLib.Factory;
using TemplateLib.Objects;

namespace TemplateLib.Builder
{
    public class CompositeBlockBuilder
    {
        private readonly Dictionary<string, string> _templateParts = new Dictionary<string, string>();
        private readonly Dictionary<string, TextBlock> _variables = new Dictionary<string, TextBlock>();

        protected readonly IConditionChecker ConditionChecker;

        public CompositeBlockBuilder(IConditionChecker conditionChecker)
        {
            ConditionChecker = conditionChecker;
        }

        public CompositeBlockBuilder Add(string variableName, string templatePart)
        {
            var checkedBlock = new TextBlock(templatePart);
            if (!ConditionChecker.Check(checkedBlock))
                return this;

            _templateParts.Add(variableName, templatePart);
            ConditionChecker.Update(checkedBlock);
            return this;
        }

        public CompositeBlockBuilder Put(string variableName, TextBlock? block)
        {
            if (block == null)
                return this;

            if (!ConditionChecker.Check(block))
                return this;

            _variables.Add(variableName, block);
            ConditionChecker.Update(block);
            return this;
        }

        public TextBlock Build()
        {
            var template = "";
            _variables.Keys.ToList().ForEach(variableName =>
            {
                if (_templateParts.TryGetValue(variableName, out string templatePart))
                    template += templatePart;
            });
            return TextBlockFactory.CreateText(template, _variables);
        }
    }
}
