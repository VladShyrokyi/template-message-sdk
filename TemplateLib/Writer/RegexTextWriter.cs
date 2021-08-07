using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using TemplateLib.Exception;

namespace TemplateLib.Writer
{
    public class RegexTextWriter : ITextWriter
    {
        private readonly string _regex;
        private readonly Func<string, string> _selectorFactory;
        private readonly Regex _selectorsPattern;
        private Dictionary<string, string> _selectors = new Dictionary<string, string>();

        private string _template = "";

        public RegexTextWriter(string template, string regex, Func<string, string> selectorFactory)
        {
            _regex = regex ?? throw new RegexNullException(this);
            _selectorFactory = selectorFactory ?? throw new ArgumentNullException(nameof(selectorFactory));
            _selectorsPattern = new Regex(_regex);
            Template = template ?? throw new TemplateNullException(this);
        }

        public RegexTextWriter(RegexTextWriter writer)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));

            _regex = writer._regex;
            _selectorFactory = writer._selectorFactory;
            _selectorsPattern = new Regex(_regex);
            Template = writer._template;
        }

        public ISet<string> Selectors => new HashSet<string>(_selectors.Keys);

        public string Template
        {
            get => _template;
            set
            {
                _selectors = new Dictionary<string, string>();
                var match = _selectorsPattern.Match(value);
                while (match.Success)
                {
                    var selectorName = match.Groups[1].Value;
                    var selectorValue = match.Value;
                    if (!_selectors.ContainsKey(selectorName))
                    {
                        _selectors.Add(selectorName, selectorValue);
                    }

                    match = match.NextMatch();
                }

                _template = value;
            }
        }

        public ITextWriter Copy()
        {
            return new RegexTextWriter(this);
        }

        public string ToWriting(Dictionary<string, string> variables, string defaultValue = "")
        {
            if (variables == null) throw new VariableNullException(this);

            var result = Template;
            var selectorsName = Selectors;
            foreach (var variable in variables)
            {
                string variableName = variable.Key;
                string variableValue = variable.Value;

                if (!selectorsName.Contains(variableName))
                    continue;

                string selector = _selectors[variableName];
                result = result.Replace(selector, variableValue);
                selectorsName.Remove(selector);
            }

            if (!_selectorsPattern.Match(result).Success)
                return result;

            foreach (var selectorName in selectorsName)
            {
                var selector = _selectors[selectorName];
                result = result.Replace(selector, defaultValue);
            }

            return result;
        }

        public string CreateSelector(string name)
        {
            return _selectorFactory.Invoke(name);
        }

        public override string ToString()
        {
            return ToWriting(new Dictionary<string, string>());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RegexTextWriter writer)) return false;
            if (writer.Template != Template) return false;
            if (writer.Selectors.Any(selector => !Selectors.Contains(selector))) return false;
            if (writer._regex != _regex) return false;
            if (!Equals(writer._selectorFactory, _selectorFactory)) return false;
            return writer.ToString() == ToString();
        }

        protected bool Equals(RegexTextWriter other)
        {
            return _regex == other._regex && _selectorFactory.Equals(other._selectorFactory) &&
                _selectorsPattern.Equals(other._selectorsPattern) && _selectors.Equals(other._selectors) &&
                _template == other._template;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _regex.GetHashCode();
                hashCode = (hashCode * 397) ^ _selectorFactory.GetHashCode();
                hashCode = (hashCode * 397) ^ _selectorsPattern.GetHashCode();
                hashCode = (hashCode * 397) ^ _selectors.GetHashCode();
                hashCode = (hashCode * 397) ^ _template.GetHashCode();
                return hashCode;
            }
        }
    }
}
