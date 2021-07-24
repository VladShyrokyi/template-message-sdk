﻿using System.Collections.Generic;
using System.Linq;

using TemplateLib.Block;
using TemplateLib.Checker;
using TemplateLib.Exception;
using TemplateLib.Factory;

namespace TemplateLib.Builder
{
    public class CompositeBlockBuilder
    {
        private readonly IConditionChecker? _conditionChecker;
        private readonly Dictionary<string, string> _templateParts = new Dictionary<string, string>();

        protected readonly Dictionary<string, ITextBlock> Variables = new Dictionary<string, ITextBlock>();

        public CompositeBlockBuilder(IConditionChecker? conditionChecker = null)
        {
            _conditionChecker = conditionChecker;
        }

        protected string Template => Variables.Keys.Aggregate(
            "",
            (template, variableName) => _templateParts.TryGetValue(variableName, out string templatePart)
                ? template + templatePart
                : template
        );

        public CompositeBlockBuilder Add(string variableName, string templatePart)
        {
            if (variableName == null) throw new VariableNameNullException(this);
            if (templatePart == null) throw new TemplateNullException(this);

            var checkedBlock = TextBlockFactory.CreateSimpleEmptyWith(templatePart);
            if (IsNotContinueAdd(checkedBlock))
                return this;

            _templateParts.Add(variableName, templatePart);
            UpdateIfCan(checkedBlock);

            return this;
        }

        public CompositeBlockBuilder Put(string variableName, ITextBlock block)
        {
            if (variableName == null) throw new VariableNameNullException(this);
            if (block == null) throw new VariableNullException(this);

            if (IsNotContinueAdd(block))
                return this;

            Variables.Add(variableName, block);
            UpdateIfCan(block);
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
