using System;
using System.Collections.Generic;

using NUnit.Framework;

using TemplateLib;
using TemplateLib.Block;
using TemplateLib.Editor;
using TemplateLib.Exception;
using TemplateLib.Writer;

namespace TemplateTest.Block
{
    [TestFixture(TestOf = typeof(TemplateBlock))]
    public class TemplateBlockTests
    {
        [Test]
        public void Write_template_with_writer([Values("", "Template")] string template)
        {
            // Arrange
            var block = CreateBlockWithoutVariables(template);

            // Act
            var text = block.Write();
            var textWithoutEditor = block.WriteWithoutEditor();

            // Assert
            Assert.AreEqual(text, template);
            Assert.AreEqual(textWithoutEditor, template);
        }


        [Test]
        public void Write_template_with_writer_and_editor([Values("", "Template")] string template,
                                                          [Values(null, "", "[start]")] string start,
                                                          [Values(null, "", "[end]")] string end)
        {
            // Arrange
            var editor = new WrapperEditor(start, end);
            var inverseEditor = new WrapperEditor(end, start);
            var block = CreateBlockWithoutVariables(template, editor);

            // Act
            var text = block.Write();
            var textWithEditor = block.WriteWithEditor(inverseEditor);
            var textWithoutEditor = block.WriteWithoutEditor();

            // Assert
            Assert.AreEqual(textWithoutEditor, template);
            Assert.AreEqual(textWithEditor, end + template + start);
            Assert.AreEqual(text, start + template + end);
        }

        [Theory]
        public void Throws_when_get_non_added_variable([Values(null, "", "0", "1", "VAR")] string variableName)
        {
            // Arrange
            var block = CreateBlockWithoutVariables("");

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
            var block = CreateBlockWithoutVariables(template, editor);
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
            var block = CreateBlockWithoutVariables(template);
            var copy = block.Copy();
            block = modification(block);

            // Asset
            Assert.That(copy, Is.AssignableTo(block.GetType()));
            Assert.AreNotEqual(copy, block);
        }

        protected static TemplateBlock CreateBlockWithoutVariables(string template, ITextEditor editor = null)
        {
            var writer = new RegexTextWriter(template, DefaultRegex.Regex, DefaultRegex.SelectorFactory);
            return new TemplateBlock(writer, editor);
        }

        protected static TemplateBlock CreateBlockWithVariables(string template,
                                                                Dictionary<string, ITextBlock> variables,
                                                                ITextEditor editor = null)
        {
            var writer = new RegexTextWriter(template, DefaultRegex.Regex, DefaultRegex.SelectorFactory);
            var block = new TemplateBlock(writer, editor);
            foreach (var pair in variables)
                block.PutVariable(pair.Key, pair.Value);
            return block;
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
