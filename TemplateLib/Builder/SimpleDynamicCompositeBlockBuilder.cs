﻿using System;
using System.Linq;
using TemplateLib.Block;
using TemplateLib.Factory;

namespace TemplateLib.Builder
{
    public class SimpleDynamicCompositeBlockBuilder : DynamicCompositeBlockBuilder
    {
        public SimpleDynamicCompositeBlockBuilder(string dynamicVariableName, string separator,
            IConditionChecker? conditionChecker = null) : base(dynamicVariableName, separator, conditionChecker)
        {
        }

        public new SimpleDynamicCompositeBlockBuilder Add(string name, string templatePart)
        {
            return (SimpleDynamicCompositeBlockBuilder) base.Add(name, templatePart);
        }

        public new SimpleDynamicCompositeBlockBuilder Put(string name, ITextBlock block)
        {
            return (SimpleDynamicCompositeBlockBuilder) base.Put(name, block);
        }

        public SimpleDynamicCompositeBlockBuilder Put(string name, string variable)
        {
            if (variable == null)
            {
                throw new ArgumentNullException(nameof(variable));
            }

            return (SimpleDynamicCompositeBlockBuilder) base.Put(name, TextBlockFactory.CreateSimpleWith(variable));
        }

        public new SimpleDynamicCompositeBlockBuilder DynamicPut(ITextBlock variable)
        {
            return (SimpleDynamicCompositeBlockBuilder) base.DynamicPut(variable);
        }

        public SimpleDynamicCompositeBlockBuilder DynamicPut(string variable)
        {
            if (variable == null)
            {
                throw new ArgumentNullException(nameof(variable));
            }

            return (SimpleDynamicCompositeBlockBuilder) base.DynamicPut(TextBlockFactory.CreateSimpleWith(variable));
        }

        public new SimpleTextBlock Build()
        {
            return TextBlockFactory.CreateSimpleWith(Template, Variables.ToDictionary(
                pair => pair.Key, pair => pair.Value.Write()
            ));
        }
    }
}
