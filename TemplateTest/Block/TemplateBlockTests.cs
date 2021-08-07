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
            TestDelegate getVariable = () => block.GetVariable(variableName);

            // Assert
            Assert.That(getVariable,
                Throws.Exception
                    .TypeOf<KeyNotFoundException>()
                    .Or
                    .TypeOf<VariableNameNullException>()
            );
        }

        [Test]
        public void Copy_template([Values("", "Template")] string template)
        {
            // Arrange
            const string addedTemplate = "added";
            const string prepend = "[start]";
            const string append = "[end]";

            // Act
            var editor = new WrapperEditor(prepend, append);
            var block = CreateBlockWithoutVariables(template, editor);
            var copy = block.Copy();

            // Assert
            Assert.AreEqual(copy, block);
            Assert.AreEqual(copy.Write(), prepend + template + append);
            Assert.AreEqual(copy.WriteWithEditor(editor), prepend + template + append);
            Assert.AreEqual(copy.WriteWithoutEditor(), template);

            // Act 2
            block.Append(addedTemplate);

            // Assert
            Assert.IsFalse(Equals(copy, block));
            Assert.IsFalse(Equals(copy.Write(), block.Write()));
            Assert.IsFalse(Equals(copy.WriteWithEditor(editor), block.WriteWithEditor(editor)));
            Assert.IsFalse(Equals(copy.WriteWithoutEditor(), block.WriteWithoutEditor()));
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
    }
}
