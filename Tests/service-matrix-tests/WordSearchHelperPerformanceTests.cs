using service_matrix.Helpers;
using Xunit;
using System.Diagnostics;

namespace service_matrix_tests
{
    public class WordSearchHelperPerformanceTests
    {
        [Fact]
        public void Search_LargeMatrix_ShouldCompleteWithinTimeLimit()
        {
            // Arrange
            var matrix = new string[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    matrix[i, j] = ((char)('a' + (i * 10 + j) % 26)).ToString();
                }
            }
            var helper = new WordSearchHelper("abcdefgh", matrix);

            // Act
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = helper.Search();
            stopwatch.Stop();

            // Assert
            Assert.True(stopwatch.ElapsedMilliseconds <= 100); // Performance check
        }

        [Fact]
        public void Search_EmptyMatrix_ShouldReturnFalse()
        {
            // Arrange
            var matrix = new string[5, 5];
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    matrix[i, j] = "";
                }
            }
            var helper = new WordSearchHelper("test", matrix);

            // Act
            var result = helper.Search();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Search_SingleLetterWord_ShouldReturnTrue()
        {
            // Arrange
            var matrix = new string[,] { { "a" } };
            var helper = new WordSearchHelper("a", matrix);

            // Act
            var result = helper.Search();

            // Assert
            Assert.True(result);
            Assert.Equal("a", helper.GetFoundString());
        }

        [Fact]
        public void Search_WordWithSpecialCharacters_ShouldHandleCorrectly()
        {
            // Arrange
            var matrix = new string[,] { { "A", "B" }, { "C", "D" } };
            var helper = new WordSearchHelper("AB", matrix);

            // Act
            var result = helper.Search();

            // Assert
            Assert.True(result);
            Assert.Equal("AB", helper.GetFoundString());
        }

        [Fact]
        public void Search_VeryLongWord_ShouldReturnFalse()
        {
            // Arrange
            var matrix = new string[5, 5];
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    matrix[i, j] = "a";
                }
            }
            var helper = new WordSearchHelper("abcdefghijk", matrix);

            // Act
            var result = helper.Search();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Search_RecursiveSearch_ShouldNotStackOverflow()
        {
            // Arrange
            var matrix = new string[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    matrix[i, j] = "a";
                }
            }
            var helper = new WordSearchHelper("aaaaaaaaaaaaa", matrix);

            // Act
            var result = helper.Search();

            // Assert
            Assert.True(result);
        }
    }
}
