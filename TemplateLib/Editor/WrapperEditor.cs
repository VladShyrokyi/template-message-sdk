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

        public override bool Equals(object obj)
        {
            return obj is WrapperEditor editor
                && editor._append == _append
                && editor._prepend == _prepend;
        }

        protected bool Equals(WrapperEditor other)
        {
            return _append == other._append && _prepend == other._prepend;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_append.GetHashCode() * 397) ^ _prepend.GetHashCode();
            }
        }
    }
}
