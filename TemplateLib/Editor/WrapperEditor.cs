namespace TemplateLib.Editor
{
    public class WrapperEditor : ITextEditor
    {
        private readonly string _append;
        private readonly string _prepend;

        public WrapperEditor(string prepend, string append)
        {
            _prepend = prepend;
            _append = append;
        }

        public ITextEditor Copy()
        {
            return new WrapperEditor(_prepend, _append);
        }

        public string ToEditing(string text)
        {
            return $"{_prepend}{text}{_append}";
        }

        public override string ToString()
        {
            return $"{_append}{_prepend}";
        }
    }
}
