using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace TemplateLib.Models
{
    public class Template
    {
        private string _template = "";
        private readonly string _regex;
        private readonly Regex _selectorsPattern;

        private Dictionary<string, string> _selectors = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _variableString = new Dictionary<string, string>();
        private readonly Dictionary<string, Template> _variableTemplates = new Dictionary<string, Template>();

        public Func<string, string>? Editor { get; set; }

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
                if (_variableString.ContainsKey(variableName) && !_variableString.Remove(variableName))
                {
                    return this;
                }
                _variableString.Add(variableName, value);
            }
            else if (typeof(T) == typeof(Template))
            {
                if (!(variableValue is Template value)) return this;

                _variableString.Remove(variableName);
                if (_variableTemplates.ContainsKey(variableName) && !_variableTemplates.Remove(variableName))
                {
                    return this;
                }
                _variableTemplates.Add(variableName, value);
            }

            return this;
        }

        public List<string> GetSelectors()
        {
            return new List<string>(_selectors.Keys);
        }

        public int GetCharCount()
        {
            return ToString().Length;
        }

        public int GetCharCountOnlyTemplate()
        {
            return Write(new Dictionary<string, string>(), "").Length;
        }

        public int GetCharCountWithoutEditor()
        {
            return Write(s => s).Length;
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

        public string Write(Func<string, string> childTemplateEditor)
        {
            var variables = _variableString
                .Concat(_variableTemplates.Select(
                        pair => new KeyValuePair<string, string>(pair.Key, pair.Value.Write(childTemplateEditor))
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

        /**
         * To write with editors if have or write without it
         */
        public override string ToString()
        {
            var variables = _variableString
                .Concat(_variableTemplates.Select(
                        pair => new KeyValuePair<string, string>(pair.Key, pair.Value.ToString())
                    )
                )
                .ToDictionary(pair => pair.Key, pair => pair.Value);
            return Editor != null ? Editor(Write(variables, "")) : Write(variables, "");
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Template template))
            {
                return false;
            }

            if (template._template != _template)
            {
                return false;
            }

            if (template.Editor != Editor)
            {
                return false;
            }

            if (template.GetSelectors().Count(s => !_selectors.ContainsKey(s)) != 0)
            {
                return false;
            }

            if (_variableString.Any(pair => template.GetVariable<string>(pair.Key) != pair.Value))
            {
                return false;
            }

            if (_variableTemplates.Any(pair => template.GetVariable<Template>(pair.Key) != pair.Value))
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _template.GetHashCode();
                hashCode = (hashCode * 397) ^ _regex.GetHashCode();
                hashCode = (hashCode * 397) ^ _selectorsPattern.GetHashCode();
                hashCode = (hashCode * 397) ^ _selectors.GetHashCode();
                hashCode = (hashCode * 397) ^ _variableString.GetHashCode();
                hashCode = (hashCode * 397) ^ _variableTemplates.GetHashCode();
                hashCode = (hashCode * 397) ^ (Editor != null ? Editor.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
