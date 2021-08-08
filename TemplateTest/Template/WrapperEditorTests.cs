using NUnit.Framework;

using TemplateLib.Editor;

namespace TemplateTest.Template
{
    [TestFixture(TestOf = typeof(WrapperEditor))]
    public class WrapperEditorTests
    {
        [Test]
        public void Edit_text([Values(null, "", "[start]")] string prepend,
                              [Values(null, "", "[end]")] string append,
                              [Values(null, "", "Text")] string text)
        {
            // Arrange
            var editor = new WrapperEditor(prepend, append);

            // Act
            var editedText = editor.ToEditing(text);

            // Assert
            Assert.AreEqual(editedText, prepend + text + append);
        }
    }
}
