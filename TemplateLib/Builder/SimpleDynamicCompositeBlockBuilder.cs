using System.Linq;

using TemplateLib.Block;
using TemplateLib.Checker;
using TemplateLib.Exception;
using TemplateLib.Factory;

namespace TemplateLib.Builder
{
    public class SimpleDynamicCompositeBlockBuilder : DynamicCompositeBlockBuilder
    {
        public SimpleDynamicCompositeBlockBuilder(string separator,
                                                  string dynamicVariableName = DefaultRegex.DynamicVariableName,
                                                  IConditionChecker? conditionChecker = null) : base(
            separator, dynamicVariableName, conditionChecker) { }

        public new SimpleDynamicCompositeBlockBuilder Add(string name, string templatePart)
        {
            if (name == null) throw new VariableNameNullException(this);
            if (templatePart == null) throw new TemplateNullException(this);

            return (SimpleDynamicCompositeBlockBuilder) base.Add(name, templatePart);
        }

        public new SimpleDynamicCompositeBlockBuilder Put(string name, ITextBlock variable)
        {
            if (name == null) throw new VariableNameNullException(this);
            if (variable == null) throw new VariableNullException(this);

            return (SimpleDynamicCompositeBlockBuilder) base.Put(name, variable);
        }

        public SimpleDynamicCompositeBlockBuilder Put(string name, string variable)
        {
            if (name == null) throw new VariableNameNullException(this);
            if (variable == null) throw new VariableNullException(this);

            return (SimpleDynamicCompositeBlockBuilder) base.Put(name, TextBlockFactory.CreateSimpleWith(variable));
        }

        public new SimpleDynamicCompositeBlockBuilder DynamicPut(ITextBlock variable)
        {
            if (variable == null) throw new VariableNullException(this);

            return (SimpleDynamicCompositeBlockBuilder) base.DynamicPut(variable);
        }

        public SimpleDynamicCompositeBlockBuilder DynamicPut(string variable)
        {
            if (variable == null) throw new VariableNullException(this);

            return (SimpleDynamicCompositeBlockBuilder) base.DynamicPut(TextBlockFactory.CreateSimpleWith(variable));
        }

        public new SimpleTextBlock Build()
        {
            return TextBlockFactory.CreateSimpleWith(
                Template,
                Variables.ToDictionary(pair => pair.Key, pair => pair.Value.Write())
            );
        }
    }
}
