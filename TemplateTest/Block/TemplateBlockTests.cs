using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using TemplateLib;
using TemplateLib.Block;
using TemplateLib.Editor;
using TemplateLib.Exception;
using TemplateLib.Writer;

using TemplateTest.Utils;

namespace TemplateTest.Block
{
    [TestFixture(TestOf = typeof(TemplateBlock))]
    public class TemplateBlockTests
    {
        private static Dictionary<string, ITextBlock>[] Variables => new[]
        {
            new Dictionary<string, ITextBlock>(),
            new Dictionary<string, ITextBlock>
            {
                {"VAR", BlockHelper.CreateInvariantBlock("Variable")}
            },
            new Dictionary<string, ITextBlock>
            {
                {"VAR_1", BlockHelper.CreateInvariantBlock("")},
                {"VAR_2", BlockHelper.CreateInvariantBlock("Variable 1")},
                {"VAR_3", BlockHelper.CreateInvariantBlock("Variable 2", new WrapperEditor("[start]", "[end]"))},
                {"VAR_4", BlockHelper.CreateInvariantBlock("Variable 3", new WrapperEditor("[end]", "[start]"))}
            },
            new Dictionary<string, ITextBlock>
            {
                {"VAR", BlockHelper.CreateTextBlock("value")}
            },
            new Dictionary<string, ITextBlock>
            {
                {"VAR_1", BlockHelper.CreateTextBlock("value 1")},
                {"VAR_2", BlockHelper.CreateTextBlock("value 2")},
                {"VAR_3", BlockHelper.CreateTextBlock("value 3", "Template with %[VAR]%")},
                {
                    "VAR_4",
                    BlockHelper.CreateTextBlock("value 4", "Edited template with %[VAR]%",
                        new WrapperEditor("[start]", "[end]"))
                },
                {
                    "VAR_5",
                    BlockHelper.CreateTextBlock("value 5", "Edited template with %[VAR]%",
                        new WrapperEditor("[start]", "[end]"))
                }
            },
            new Dictionary<string, ITextBlock>
            {
                {
                    "VAR", BlockHelper.CreateTemplateBlock("Variable:\n%[VAR_1]% and %[VAR_2]%",
                        new Dictionary<string, ITextBlock>
                        {
                            {"VAR_1", BlockHelper.CreateInvariantBlock("Template 1 in variable\n")},
                            {"VAR_2", BlockHelper.CreateTextBlock("variable", "Template 2 in %[VAR]%\n")}
                        })
                }
            },
            new Dictionary<string, ITextBlock>
            {
                {
                    "VAR_1", BlockHelper.CreateTemplateBlock("Variable 1:\n%[VAR_1]% and %[VAR_2]%",
                        new Dictionary<string, ITextBlock>
                        {
                            {"VAR_1", BlockHelper.CreateInvariantBlock("Template 1 in variable 1")},
                            {"VAR_2", BlockHelper.CreateInvariantBlock("Template 2 in variable 1")}
                        })
                },
                {
                    "VAR_2", BlockHelper.CreateTemplateBlock("Variable 2:\n %[VAR_1]% and %[VAR_2]%",
                        new Dictionary<string, ITextBlock>
                        {
                            {"VAR_1", BlockHelper.CreateInvariantBlock("Template 1 in variable 2")},
                            {"VAR_2", BlockHelper.CreateInvariantBlock("Template 2 in variable 2")}
                        })
                }
            }
        };

        [Test]
        public void Write_template([Values("", "Template")] string template)
        {
            // Arrange
            var block = BlockHelper.CreateTemplateBlock(template);

            // Act
            var text = block.Write();
            var textWithoutEditor = block.WriteWithoutEditor();

            // Assert
            Assert.AreEqual(text, template);
            Assert.AreEqual(textWithoutEditor, template);
        }

        [TestCaseSource(nameof(Variables))]
        public void Write_template_with_variables(Dictionary<string, ITextBlock> variables)
        {
            // Arrange
            var variableNameToSelector =
                variables.ToDictionary(pair => pair.Key, pair => DefaultRegex.SelectorFactory(pair.Key));
            var template = TextHelper.CreateTextWithVariables(variableNameToSelector);
            var block = BlockHelper.CreateTemplateBlock(template, variables);

            // Act
            var text = block.Write();
            var textWithoutEditor = block.WriteWithoutEditor();

            // Assert
            var variablesWrite = variables.ToDictionary(pair => pair.Key, pair => pair.Value.Write());
            var result = TextHelper.CreateTextWithVariables(variablesWrite);
            Assert.AreEqual(text, result);

            var variablesWriteWithoutEditor =
                variables.ToDictionary(pair => pair.Key, pair => pair.Value.WriteWithoutEditor());
            var resultWithoutEditor = TextHelper.CreateTextWithVariables(variablesWriteWithoutEditor);
            Assert.AreEqual(textWithoutEditor, resultWithoutEditor);
        }

        [Test]
        public void Write_and_edit_template([Values("", "Template")] string template,
                                            [Values(null, "", "[start]")] string start,
                                            [Values(null, "", "[end]")] string end)
        {
            // Arrange
            var editor = new WrapperEditor(start, end);
            var inverseEditor = new WrapperEditor(end, start);
            var block = BlockHelper.CreateTemplateBlock(template, null, editor);

            // Act
            var text = block.Write();
            var textWithEditor = block.WriteWithEditor(inverseEditor);
            var textWithoutEditor = block.WriteWithoutEditor();

            // Assert
            Assert.AreEqual(textWithoutEditor, template);
            Assert.AreEqual(textWithEditor, end + template + start);
            Assert.AreEqual(text, start + template + end);
        }

        [Test]
        public void Append_template([Values("", "Template")] string template,
                                    [Values("", " append")] string templateAppend)
        {
            var block = BlockHelper.CreateTemplateBlock(template);
            block.Append(templateAppend);

            Assert.AreEqual(block.Writer.Template, template + templateAppend);
        }

        [Theory]
        public void Throws_when_get_non_added_variable([Values(null, "", "0", "1", "VAR")] string variableName)
        {
            // Arrange
            var block = BlockHelper.CreateTemplateBlock("");

            // Act
            var getVariable = new TestDelegate(() => block.GetVariable(variableName));

            // Assert
            Assert.That(getVariable,
                Throws.Exception
                    .TypeOf<KeyNotFoundException>()
                    .Or
                    .TypeOf<VariableNameNullException>()
            );
        }

        [Test]
        public void Copy_equals_source([Values("", "Template")] string template)
        {
            // Arrange
            const string prepend = "[start]";
            const string append = "[end]";
            var editor = new WrapperEditor(prepend, append);
            var inverseEditor = new WrapperEditor(append, prepend);

            // Act
            var block = BlockHelper.CreateTemplateBlock(template, null, editor);
            var copy = block.Copy();

            // Assert
            Assert.AreEqual(copy.Write(), block.Write());
            Assert.AreEqual(copy.WriteWithEditor(inverseEditor), block.WriteWithEditor(inverseEditor));
            Assert.AreEqual(copy.WriteWithoutEditor(), block.WriteWithoutEditor());
            Assert.AreEqual(copy, block);
        }

        [TestCaseSource(nameof(Modifications))]
        public void Copy_not_equals_modified_source(Func<TemplateBlock, TemplateBlock> modification)
        {
            // Arrange
            const string template = "Template";

            // Act
            var block = BlockHelper.CreateTemplateBlock(template);
            var copy = block.Copy();
            block = modification(block);

            // Asset
            Assert.That(copy, Is.AssignableTo(block.GetType()));
            Assert.AreNotEqual(copy, block);
        }

        private static Func<TemplateBlock, TemplateBlock>[] Modifications()
        {
            const string modificationText = "\nBy modification";
            return new Func<TemplateBlock, TemplateBlock>[]
            {
                block =>
                {
                    block.Editor = new WrapperEditor("", modificationText);
                    return block;
                },
                block =>
                {
                    block.Append(modificationText);
                    return block;
                },
                block =>
                {
                    const string variableName = "VAR";
                    var writer = new RegexTextWriter(modificationText, DefaultRegex.Regex,
                        DefaultRegex.SelectorFactory);
                    var variableValue = new InvariantBlock(writer, null);
                    block.PutVariable(variableName, variableValue);
                    return block;
                }
            };
        }
    }
}
