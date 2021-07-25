using TemplateLib.Block;
using TemplateLib.Checker;
using TemplateLib.Editor;
using TemplateLib.Exception;
using TemplateLib.Factory;
using TemplateLib.Writer;

namespace TemplateLib.Builder
{
    public class TemplateBlockConditionBuilder : TemplateBlockBuilder
    {
        private readonly IConditionChecker? _conditionChecker;

        public TemplateBlockConditionBuilder(ITextWriter writer, ITextEditor? editor,
                                             IConditionChecker? conditionChecker = null) : base(writer, editor)
        {
            _conditionChecker = conditionChecker;
        }

        public new void Append(string templatePart)
        {
            TryAppend(templatePart);
        }

        public bool TryAppend(string templatePart)
        {
            if (templatePart == null) throw new TemplateNullException(this);
            var template = TextBlockFactory.CreateOnlyTemplate(templatePart);
            if (IsNotContinueBuild(template)) return false;

            base.Append(templatePart);
            UpdateIfCan(template);
            return true;
        }

        public new void PutVariable(string name, ITextBlock variable)
        {
            TryPutVariable(name, variable);
        }

        public bool TryPutVariable(string name, ITextBlock variable)
        {
            if (name == null) throw new VariableNameNullException(this);
            if (variable == null) throw new VariableNullException(this);
            if (IsNotContinueBuild(variable)) return false;

            base.PutVariable(name, variable);
            UpdateIfCan(variable);
            return true;
        }

        protected bool IsNotContinueBuild(ITextBlock checkedBlock)
        {
            return _conditionChecker != null && !_conditionChecker.Check(checkedBlock);
        }

        protected void UpdateIfCan(ITextBlock checkedBlock)
        {
            _conditionChecker?.Update(checkedBlock);
        }
    }
}
