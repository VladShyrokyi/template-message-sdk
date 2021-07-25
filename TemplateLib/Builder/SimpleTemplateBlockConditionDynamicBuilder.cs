using System.Linq;

using TemplateLib.Block;
using TemplateLib.Checker;
using TemplateLib.Exception;
using TemplateLib.Factory;

namespace TemplateLib.Builder
{
    public class SimpleTemplateBlockConditionDynamicBuilder : TemplateBlockConditionDynamicBuilder
    {
        public SimpleTemplateBlockConditionDynamicBuilder(string separator,
                                                  string dynamicVariableName = DefaultRegex.DynamicVariableName,
                                                  IConditionChecker? conditionChecker = null) : base(
            separator, dynamicVariableName, conditionChecker) { }

        public new SimpleTemplateBlockConditionDynamicBuilder Add(string name, string templatePart)
        {
            if (name == null) throw new VariableNameNullException(this);
            if (templatePart == null) throw new TemplateNullException(this);

            return (SimpleTemplateBlockConditionDynamicBuilder) base.Add(name, templatePart);
        }

        public new SimpleTemplateBlockConditionDynamicBuilder Put(string name, ITextBlock variable)
        {
            if (name == null) throw new VariableNameNullException(this);
            if (variable == null) throw new VariableNullException(this);

            return (SimpleTemplateBlockConditionDynamicBuilder) base.Put(name, variable);
        }

        public SimpleTemplateBlockConditionDynamicBuilder Put(string name, string variable)
        {
            if (name == null) throw new VariableNameNullException(this);
            if (variable == null) throw new VariableNullException(this);

            return (SimpleTemplateBlockConditionDynamicBuilder) base.Put(name, TextBlockFactory.CreateSimpleWith(variable));
        }

        public new SimpleTemplateBlockConditionDynamicBuilder DynamicPut(ITextBlock variable)
        {
            if (variable == null) throw new VariableNullException(this);

            return (SimpleTemplateBlockConditionDynamicBuilder) base.DynamicPut(variable);
        }

        public SimpleTemplateBlockConditionDynamicBuilder DynamicPut(string variable)
        {
            if (variable == null) throw new VariableNullException(this);

            return (SimpleTemplateBlockConditionDynamicBuilder) base.DynamicPut(TextBlockFactory.CreateSimpleWith(variable));
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
