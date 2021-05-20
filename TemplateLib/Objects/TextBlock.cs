using System;
using System.Collections.Generic;

namespace TemplateLib.Objects
{
    public class TextBlock
    {
        private List<string> _variablesName;

        public Template Template { get; }

        public TextBlock(string template)
        {
            Template = new Template(template);
            _variablesName = Template.GetSelectors();
        }

        public TextBlock(string template, Dictionary<string, string> variables) : this(template)
        {
            PutVariables(variables);
        }

        public TextBlock(string template, Dictionary<string, string> variables, Func<string, string> editor) : this(template, variables)
        {
            SetTemplateEditor(editor);
        }

        public TextBlock SetTemplate(string templateString)
        {
            Template.TemplateString = templateString;
            _variablesName = Template.GetSelectors();

            return this;
        }

        public Template CopyTemplate()
        {
            var copyTemplate = new Template(Template.TemplateString);
            _variablesName.ForEach(variableName =>
            {
                var variablesTemplate = Template.GetVariable<Template>(variableName);
                if (variablesTemplate != null)
                {
                    copyTemplate.PutVariable(variableName, variablesTemplate);
                }
                else if (Template.GetVariable<string>(variableName) is { } variableString)
                {
                    copyTemplate.PutVariable(variableName, variableString);
                }
            });
            return copyTemplate;
        }

        public TextBlock SetTemplateEditor(Func<string, string> editor)
        {
            Template.Editor = editor;

            return this;
        }

        public Func<string, string>? GetTemplateEditor()
        {
            return Template.Editor;
        }

        public TextBlock PutVariable<T>(string variableName, T? variableValue) where T : class
        {
            if (variableValue == null || typeof(T) != typeof(string) || typeof(T) != typeof(Template) ||
                typeof(T) != typeof(TextBlock))
            {
                return this;
            }

            switch (variableValue)
            {
                case TextBlock value:
                    Template.PutVariable(variableName, value.CopyTemplate());
                    break;
                case string value:
                    Template.PutVariable(variableName, value);
                    break;
            }

            return this;
        }

        public TextBlock PutVariables<T>(Dictionary<string, T> variables) where T : class
        {
            foreach (var pair in variables)
            {
                var variableName = pair.Key ?? throw new ArgumentNullException(nameof(variables));
                var variableValue = pair.Value;

                PutVariable(variableName, variableValue);
            }

            return this;
        }
    }
}
