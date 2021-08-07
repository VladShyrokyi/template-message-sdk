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
