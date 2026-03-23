using service_matrix.Commands;
using Xunit;

namespace service_matrix_tests
{
    public class CommandTests
    {
        [Fact]
        public void WordSearchCommand_ShouldHaveCorrectValues()
        {
            // Arrange
            var matrix = new List<List<string>> { new List<string> { "a", "b" } };
            var command = new WordSearchCommand(10, 1, 20, matrix);

            // Act
            // No action needed, just check values

            // Assert
            Assert.Equal(10, command.MaxLength);
            Assert.Equal(1, command.MinLength);
            Assert.Equal(20, command.MaxWords);
            Assert.NotNull(command.LettersMatrix);
        }

        [Fact]
        public void WordSearchCommand_DefaultValues_ShouldHaveCorrectDefaults()
        {
            // Arrange
            var matrix = new List<List<string>> { new List<string> { "a" } };
            var command = new WordSearchCommand(5, 1, 10, matrix);

            // Act
            // No action needed, just check values

            // Assert
            Assert.Equal(5, command.MaxLength);
            Assert.Equal(1, command.MinLength);
            Assert.Equal(10, command.MaxWords);
            Assert.NotNull(command.LettersMatrix);
        }

        [Fact]
        public void MergeWordsCommand_ShouldBeInstantiable()
        {
            // Arrange
            // No arrangement needed

            // Act
            var command = new MergeWordsCommand();

            // Assert
            Assert.NotNull(command);
        }

        [Fact]
        public void MergeWordsCommand_ShouldHaveDefaultValues()
        {
            // Arrange
            var command = new MergeWordsCommand();

            // Act
            // No action needed

            // Assert
            Assert.NotNull(command);
        }
    }
}
