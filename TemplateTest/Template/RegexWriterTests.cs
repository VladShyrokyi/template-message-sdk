using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using TemplateLib;
using TemplateLib.Writer;

using TemplateTest.Utils;

namespace TemplateTest.Template
{
    [TestFixture(TestOf = typeof(RegexTextWriter))]
    public class RegexWriterTests
    {
        private static object[] Regex => new object[]
        {
            new object[] {DefaultRegex.Regex, DefaultRegex.SelectorFactory},
            new object[] {"\\{\\{([^\\{\\},\\s]*)\\}\\}", new Func<string, string>(name => $"{{{{{name}}}}}")}
        };

        private static Dictionary<string, string>[] Variables => new[]
        {
            new Dictionary<string, string>
            {
                {"VAR", ""}
            },
            new Dictionary<string, string>
            {
                {"VAR_1", ""},
                {"VAR_2", "Test"},
                {"VAR_3", "Test 1"},
                {"VAR_4", "1"},
                {"VAR_5", "Test $@#123!$"},
                {"VAR_6", "Alert"}
            }
        };

        [Test]
        public void Create_selector([Values(null, "", "SELECTOR")] string selectorName)
        {
            // Arrange
            var writer = new RegexTextWriter("", DefaultRegex.Regex, DefaultRegex.SelectorFactory);

            // Act
            var selector = writer.CreateSelector(selectorName);

            // Assert
            Assert.AreEqual(selector, DefaultRegex.SelectorFactory(selectorName));
        }

        [TestCaseSource(nameof(Regex))]
        public void Not_found_selectors_in_template(string regex, Func<string, string> selectorFactory)
        {
            // Arrange
            const string template = "Template";
            const string variableName = "VAR";
            const string variableValue = "Text";
            var variables = new Dictionary<string, string> {{variableName, variableValue}};

            // Act
            var writer = new RegexTextWriter(template, regex, selectorFactory);
            var text = writer.ToWriting(variables);

            // Assert
            Assert.AreEqual(writer.Template, template);
            Assert.IsEmpty(writer.Selectors);
            StringAssert.DoesNotContain(variableName, text);
            StringAssert.DoesNotContain(variableValue, text);
        }

        [TestCaseSource(nameof(Regex))]
        public void Found_selectors_in_template(string regex, Func<string, string> selectorFactory)
        {
            // Arrange
            const string variableName = "SELECTOR";
            const string variableValue = "Text";
            var variables = new Dictionary<string, string> {{variableName, variableValue}};
            var variableNameToSelector = variables.ToDictionary(pair => pair.Key, pair => selectorFactory(pair.Key));
            var template = TextHelper.CreateTextWithVariables(variableNameToSelector);

            // Act
            var writer = new RegexTextWriter(template, regex, selectorFactory);
            var text = writer.ToWriting(variables);

            // Assert
            Assert.AreEqual(writer.Template, template);
            Assert.IsNotEmpty(writer.Selectors);
            CollectionAssert.Contains(writer.Selectors, variableName);
            StringAssert.Contains(variableName, text);
            StringAssert.Contains(variableValue, text);
        }

        [Test]
        public void Write_text_with_default_variable([Values(null, "", "Default")] string variableValue)
        {
            // Arrange
            const string variableName = "DEFAULT";
            var variables = new Dictionary<string, string>
            {
                {variableName, variableValue}
            };
            var variableNameToSelector =
                variables.ToDictionary(pair => pair.Key, pair => DefaultRegex.SelectorFactory(pair.Key));
            var template = TextHelper.CreateTextWithVariables(variableNameToSelector);

            // Act
            var writer = new RegexTextWriter(template, DefaultRegex.Regex, DefaultRegex.SelectorFactory);
            var text = writer.ToWriting(new Dictionary<string, string>(), variableValue);

            // Assert
            var result = TextHelper.CreateTextWithVariables(variables);
            Assert.AreEqual(text, result);
        }

        [TestCaseSource(nameof(Variables))]
        public void Write_text_with_variables(Dictionary<string, string> variables)
        {
            // Arrange
            var variableNameToSelector = variables
                .ToDictionary(pair => pair.Key, pair => DefaultRegex.SelectorFactory(pair.Key));
            var template = TextHelper.CreateTextWithVariables(variableNameToSelector);

            // Act
            var writer = new RegexTextWriter(template, DefaultRegex.Regex, DefaultRegex.SelectorFactory);
            var text = writer.ToWriting(variables);

            // Assert
            Assert.AreEqual(writer.Template, template);
            Assert.IsNotEmpty(writer.Selectors);
            Assert.AreEqual(writer.Selectors.Count, variables.Count);

            var result = TextHelper.CreateTextWithVariables(variables);
            Assert.AreEqual(text, result);
        }

        [Theory]
        [TestCaseSource(nameof(Variables))]
        public void Copy_equals_source(Dictionary<string, string> variables)
        {
            //Arrange
            const string defaultValue = "DEFAULT";
            var variableNameToSelector =
                variables.ToDictionary(pair => pair.Key, pair => DefaultRegex.SelectorFactory(pair.Key));
            var template = TextHelper.CreateTextWithVariables(variableNameToSelector);

            // Act
            var writer = new RegexTextWriter(template, DefaultRegex.Regex, DefaultRegex.SelectorFactory);
            var copy = writer.Copy();

            // Assert
            Assert.AreEqual(copy.Template, writer.Template);
            Assert.AreEqual(copy.ToWriting(variables), writer.ToWriting(variables));
            Assert.AreEqual(copy.ToWriting(variables, defaultValue), writer.ToWriting(variables, defaultValue));
            Assert.That(copy, Is.AssignableTo(writer.GetType()));
            Assert.AreEqual(copy, writer);
        }
    }
}
