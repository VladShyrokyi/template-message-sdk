using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace TemplateLib.Objects
{
    [AttributeUsage(AttributeTargets.All)]
    public class Template
    {
        private string _template;
        private readonly string _regex;
        private readonly Regex _selectorsPattern;


        private Dictionary<string, string> _selectors = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _variableString = new Dictionary<string, string>();
        private readonly Dictionary<string, Template> _variableTemplates = new Dictionary<string, Template>();

        private Func<string, string>? _editor;

        public Template(string template, string regex, Func<string, string>? editor)
        {
            _regex = regex;
            _selectorsPattern = new Regex(_regex);
            Editor = editor;

            TemplateString = template;
        }

        public Template(string template, string regex) : this(template, regex, null)
        {
        }

        public Template(string template) : this(template, "%\\[([^%,\\s]+)\\]%")
        {
        }

        public string TemplateString
        {
            get => _template;
            set
            {
                _selectors = new Dictionary<string, string>();
                var match = _selectorsPattern.Match(value);
                while (match.Success)
                {
                    _selectors.Add(match.Groups[0].Value, match.Value);
                    match.NextMatch();
                }

                _template = value;
            }
        }

        public T? GetVariable<T>(string? variableName) where T : class
        {
            if (variableName == null)
            {
                return null;
            }

            if (typeof(T) == typeof(string))
            {
                return _variableString.TryGetValue(variableName, out var valueString)
                    ? valueString as T
                    : null;
            }

            if (typeof(T) == typeof(Template))
            {
                return _variableTemplates.TryGetValue(variableName, out var valueTemplate)
                    ? valueTemplate as T
                    : null;
            }

            return null;
        }

        public Template PutVariable<T>(string variableName, T variableValue) where T : class
        {
            if (typeof(T) == typeof(string))
            {
                if (!(variableValue is string value)) return this;

                _variableTemplates.Remove(variableName);
                _variableString.Add(variableName, value);
            }
            else if (typeof(T) == typeof(Template))
            {
                if (!(variableValue is Template value)) return this;

                _variableString.Remove(variableName);
                _variableTemplates.Add(variableName, value);
            }

            return this;
        }

        public List<string> GetSelectors()
        {
            return new List<string>(_selectors.Keys);
        }

        public string Write()
        {
            var variables = _variableString
                .Concat(_variableTemplates.Select(
                        pair => new KeyValuePair<string, string>(pair.Key, pair.Value.Write())
                    )
                )
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            return Write(variables, "");
        }

        public string Write(string emptyVariables)
        {
            var variables = _variableString
                .Concat(_variableTemplates.Select(pair =>
                        new KeyValuePair<string, string>(pair.Key, pair.Value.Write(emptyVariables))
                    )
                )
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            return Write(variables, emptyVariables);
        }

        public string Write(Dictionary<string, string> variables, string emptyVariables)
        {
            var result = _template;
            var selectorsName = GetSelectors();
            foreach (var variable in variables)
            {
                string variableName = variable.Key;
                string variableValue = variable.Value;

                if (!selectorsName.Contains(variableName))
                {
                    break;
                }

                string selector = _selectors[variableName];
                result = result.Replace(selector, variableValue);
                selectorsName.Remove(selector);
            }

            if (!_selectorsPattern.Match(result).Success)
            {
                return result;
            }

            selectorsName.ForEach(selectorName =>
            {
                string selector = _selectors[selectorName];
                result = result.Replace(selector, emptyVariables);
            });

            return result;
        }

        public override string ToString()
        {
            return Editor != null ? Editor(Write()) : Write(_variableString, "");
        }
    }
}
