using System.Collections.Generic;
using System.Linq;

using TemplateLib.Block;
using TemplateLib.Checker;
using TemplateLib.Exception;
using TemplateLib.Factory;

namespace TemplateLib.Builder
{
    public class TemplateBlockConditionBuilder
    {
        private readonly IConditionChecker? _conditionChecker;
        private readonly Dictionary<string, string> _templateParts = new Dictionary<string, string>();

        protected readonly Dictionary<string, ITextBlock> Variables = new Dictionary<string, ITextBlock>();

        public TemplateBlockConditionBuilder(IConditionChecker? conditionChecker = null)
        {
            _conditionChecker = conditionChecker;
        }

        protected string Template => Variables.Keys.Aggregate(
            "",
            (template, variableName) => _templateParts.TryGetValue(variableName, out string templatePart)
                ? template + templatePart
                : template
        );

        public TemplateBlockConditionBuilder Add(string name, string templatePart)
        {
            if (name == null) throw new VariableNameNullException(this);
            if (templatePart == null) throw new TemplateNullException(this);

            var checkedBlock = TextBlockFactory.CreateSimpleEmptyWith(templatePart);
            if (IsNotContinueAdd(checkedBlock))
                return this;

            _templateParts.Add(name, templatePart);
            UpdateIfCan(checkedBlock);

            return this;
        }

        public TemplateBlockConditionBuilder Put(string name, ITextBlock variable)
        {
            if (name == null) throw new VariableNameNullException(this);
            if (variable == null) throw new VariableNullException(this);

            if (IsNotContinueAdd(variable))
                return this;

            Variables.Add(name, variable);
            UpdateIfCan(variable);
            return this;
        }

        public TemplateTextBlock Build()
        {
            return TextBlockFactory.CreateTemplateWith(Template, Variables);
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
