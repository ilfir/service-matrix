using service_matrix.Helpers;
using Xunit;

namespace service_matrix.Tests.Helpers
{
    public class WordSearchHelperTests
    {
        [Fact]
        public void CopyArray_ShouldReturnExactCopy()
        {
            // Arrange
            var word = "test";
            string[,] source =
            {
                {"a", "b", "c", "b", "c"},
                {"d", "t", "f", "b", "c"},
                {"g", "h", "s", "b", "c"},
                {"g", "h", "i", "e", "c"},
                {"g", "h", "i", "b", "t"}
            };
            var helper = new WordSearchHelper("word", source);

            // Act
            var result = helper.CopyArray(source);

            // Assert
            Assert.Equal(source, result);
            Assert.False(helper.Search());
            Assert.Equal(string.Empty, helper.GetFoundString());
        }

        [Fact]
        public void IsAllLettersInMatrix_ShouldReturnTrue_WhenAllLettersArePresent()
        {
            // Arrange
            string word = "doc";
            string[,] source =
            {
                {"a", "b", "c", "b", "c"},
                {"d", "e", "f", "b", "c"},
                {"g", "d", "o", "c", "c"},
                {"g", "h", "i", "b", "c"},
                {"g", "h", "i", "b", "c"}
            };
            var helper = new WordSearchHelper(word, source);

            // Act
            var result = helper.IsAllLettersInMatrix(source, word);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsAllLettersInMatrix_ShouldReturnFalse_WhenNotAllLettersArePresent()
        {
            // Arrange
            string word = "xyz";
            string[,] source =
            {
                {"a", "b", "c", "b", "c"},
                {"d", "e", "f", "b", "c"},
                {"g", "d", "o", "c", "c"},
                {"g", "h", "i", "b", "c"},
                {"g", "h", "i", "b", "c"}
            };
            var helper = new WordSearchHelper(word, source);


            // Act
            var result = helper.IsAllLettersInMatrix(source, word);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsNeighborToNextLetter_ShouldReturnTrue_WhenNextLetterIsNeighbor()
        {
            // Arrange
            string[,] source =
            {
                {"a", "b", "c", "b", "c"},
                {"d", "e", "f", "b", "c"},
                {"g", "d", "o", "c", "c"},
                {"g", "h", "i", "b", "c"},
                {"g", "h", "i", "b", "c"}
            };
            var helper = new WordSearchHelper("ab", source);
            string[] word = {"a", "b"};

            // Act
            var result = helper.IsNeighborToNextLetter(0, 0, word, 0, source);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsNeighborToNextLetter_ShouldReturnFalse_WhenNextLetterIsNotNeighbor()
        {
            // Arrange
            string[,] source =
            {
                {"a", "b", "c", "b", "c"},
                {"d", "e", "f", "b", "c"},
                {"g", "d", "o", "c", "c"},
                {"g", "h", "i", "b", "c"},
                {"g", "h", "i", "b", "c"}
            };
            var helper = new WordSearchHelper("ab", source);
            string[] word = {"a", "b"};

            // Act
            var result = helper.IsNeighborToNextLetter(0, 0, word, 0, source);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void WordSearch_Multiple_ShouldReturnTrue_жирнеть()
        {
            // Arrange
            string word = "жирнеть";
            string[,] source =
            {
                {"ж", "и", "р", "b", "c"},
                {"р", "н", "е", "т", "ь"},
                {"е", "d", "з", "c", "c"},
                {"в", "h", "i", "b", "c"},
                {"з", "h", "i", "b", "c"}
            };
            var helper = new WordSearchHelper(word, source);

            // Act
            var result = helper.Search();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void WordSearch_Multiple_ShouldReturnTrue_сдвижение()
        {
            // Arrange
            string word = "сдвижение";
            string[,] source = {
                {"с", "", "", "", "",},
                {"д", "в", "и", "", ""},
                {"с", "ж", "е", "", ""},
                {"", "н", "и", "е", ""},
                {"", "", "", "", ""}
            };

            var helper = new WordSearchHelper(word, source);

            // Act
            var result = helper.Search();

            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void WordSearch_Multiple_ShouldReturnTrue_дарвинист()
        {
            // Arrange
            string word = "дарвинист";
            string[,] source = {
                {"е", "в", "с", "е", "ю"},
                {"н", "и", "з", "т", "т"},
                {"н", "и", "к", "п", "ю"},
                {"е", "в", "р", "д", "о"},
                {"м", "л", "о", "а", "и"}
            };

            var helper = new WordSearchHelper(word, source);

            // Act
            var result = helper.Search();

            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void WordSearch_Multiple_StartPoints_ShouldReturnTrue()
        {
            // Arrange
            string word = "зверь";
            string[,] source = {
                { "ь", "b", "з", "b", "c" },
                { "р", "e", "f", "b", "c" },
                { "е", "d", "з", "c", "c" },
                { "в", "h", "i", "b", "c" },
                { "з", "h", "i", "b", "c" }
            };
            var helper = new WordSearchHelper(word, source);

            // Act
            var result = helper.Search();

            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void WordSearch_Multiple_StartPoints_ShouldReturnTrue_3()
        {
            // Arrange
            string word = "растлеваться";
            string[,] source = {
                { "р", "а", "с", "b", "п" },
                { "п", "т", "л", "е", "c" },
                { "е", "н", "в", "а", "c" },
                { "е", "ж", "т", "ь", "c" },
                { "п", "п", "с", "я", "д" }
            };
            var helper = new WordSearchHelper(word, source);

            // Act
            var result = helper.Search();

            // Assert
            Assert.True(result);
        }
           
        [Fact]
        public void WordSearch_Multiple_StartPoints_ShouldReturnTrue_9()
        {
            // Arrange
            string word = "развевание";
            string[,] source = {
                { "р", "а", "з", "b", "п" },
                { "п", "в", "е", "b", "c" },
                { "е", "в", "а", "н", "c" },
                { "е", "ж", "о", "и", "c" },
                { "п", "п", "р", "е", "д" }
            };
            var helper = new WordSearchHelper(word, source);

            // Act
            var result = helper.Search();

            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void WordSearch_Multiple_StartPoints_ShouldReturnTrue_2()
        {
            // Arrange
            string word = "предложение";
            string[,] source = {
                { "п", "b", "з", "b", "п" },
                { "п", "e", "f", "b", "c" },
                { "е", "н", "и", "е", "c" },
                { "е", "ж", "о", "л", "c" },
                { "п", "п", "р", "е", "д" }
            };
            var helper = new WordSearchHelper(word, source);

            // Act
            var result = helper.Search();

            // Assert
            Assert.True(result);
        }

        
        [Fact]
        public void WordSearch_FindAllStartPoints_ShouldReturn3Locations()
        {
            // Arrange
            string word = "зверь";
            string[,] source = {
                { "ь", "b", "з", "b", "c" },
                { "р", "e", "f", "b", "c" },
                { "е", "d", "з", "c", "c" },
                { "в", "h", "i", "b", "c" },
                { "з", "h", "i", "b", "c" }
            };
            var helper = new WordSearchHelper(word, source);

            // Act
            var result = helper.FindLetterLocations();

            // Assert
            Assert.True(result.Count == 3);
        }
    }
}