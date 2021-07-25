using TemplateLib.Block;
using TemplateLib.Editor;
using TemplateLib.Exception;
using TemplateLib.Writer;

namespace TemplateLib.Builder
{
    public class TemplateBlockDynamicBuilder : TemplateBlockBuilder
    {
        protected new readonly RegexTextWriter Writer;

        private readonly string _separator;
        private readonly string _dynamicVariableName;

        private int _dynamicVariableCounter;

        public TemplateBlockDynamicBuilder(RegexTextWriter writer, ITextEditor? editor,
                                           string separator, string dynamicVariableName) : base(writer, editor)
        {
            Writer = writer;
            _separator = separator;
            _dynamicVariableName = dynamicVariableName;
        }

        public void Append(ITextBlock block)
        {
            if (block == null) throw new VariableNullException(this);

            var variableName = _dynamicVariableName + "_" + _dynamicVariableCounter;
            var templatePart = _dynamicVariableCounter == 0
                ? Writer.CreateSelector(variableName)
                : _separator + Writer.CreateSelector(variableName);

            Append(templatePart);
            PutVariable(variableName, block);
            _dynamicVariableCounter++;
        }
    }
}
