using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TemplateLib.Writer
{
    public class RegexTextWriter : ITextWriter
    {
        private readonly string _regex;
        private readonly Regex _selectorsPattern;
        private Dictionary<string, string> _selectors = new Dictionary<string, string>();

        private string _template = "";

        public RegexTextWriter(string template, string regex)
        {
            _regex = regex;
            _selectorsPattern = new Regex(_regex);
            Template = template;
        }

        public RegexTextWriter(RegexTextWriter writer)
        {
            _regex = writer._regex;
            _selectorsPattern = new Regex(_regex);
            Template = writer._template;
        }

        public ISet<string> Selectors => _selectors.Keys.ToHashSet();

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

        public string ToWriting(Dictionary<string, string> variables, string defaultValue)
        {
            var result = Template;
            var selectorsName = Selectors;
            foreach (var variable in variables)
            {
                string variableName = variable.Key;
                string variableValue = variable.Value;

                if (!selectorsName.Contains(variableName))
                {
                    continue;
                }

                string selector = _selectors[variableName];
                result = result.Replace(selector, variableValue);
                selectorsName.Remove(selector);
            }

            if (!_selectorsPattern.Match(result).Success)
            {
                return result;
            }

            foreach (var selectorName in selectorsName)
            {
                var selector = _selectors[selectorName];
                result = result.Replace(selector, defaultValue);
            }

            return result;
        }
    }
}