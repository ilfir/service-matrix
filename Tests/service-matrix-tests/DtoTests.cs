using service_matrix.DTO;
using Xunit;

namespace service_matrix_tests
{
    public class DtoTests
    {
        [Fact]
        public void SearchRequest_DefaultValues_ShouldHaveCorrectDefaults()
        {
            // Arrange
            var request = new SearchRequest();

            // Act
            // No action needed, just check defaults

            // Assert
            Assert.Equal(5, request.MaxLength);
            Assert.Equal(10, request.MaxWords);
            Assert.Equal(1, request.MinLength);
            Assert.Null(request.LettersMatrix);
        }

        [Fact]
        public void SearchRequest_CustomValues_ShouldSetCorrectly()
        {
            // Arrange
            var request = new SearchRequest
            {
                MaxLength = 10,
                MinLength = 2,
                MaxWords = 20,
                LettersMatrix = new List<List<string>> { new List<string> { "a", "b" } }
            };

            // Act
            // No action needed, just check values

            // Assert
            Assert.Equal(10, request.MaxLength);
            Assert.Equal(2, request.MinLength);
            Assert.Equal(20, request.MaxWords);
            Assert.NotNull(request.LettersMatrix);
        }

        [Fact]
        public void UpdateWordsRequest_DefaultValues_ShouldHaveCorrectDefaults()
        {
            // Arrange
            var request = new UpdateWordsRequest();

            // Act
            // No action needed, just check defaults

            // Assert
            Assert.Empty(request.Words);
            Assert.False(request.Include); // Default value
        }

        [Fact]
        public void UpdateWordsRequest_CustomValues_ShouldSetCorrectly()
        {
            // Arrange
            var request = new UpdateWordsRequest
            {
                Words = new List<string> { "word1", "word2" },
                Include = true
            };

            // Act
            // No action needed, just check values

            // Assert
            Assert.Equal(2, request.Words.Count);
            Assert.Equal("word1", request.Words[0]);
            Assert.Equal("word2", request.Words[1]);
            Assert.True(request.Include);
        }

        [Fact]
        public void MergeResponse_ShouldHaveCorrectValues()
        {
            // Arrange
            var response = new MergeResponse(10, 5);

            // Act
            // No action needed, just check values

            // Assert
            Assert.Equal(10, response.AddedCount);
            Assert.Equal(5, response.RemovedCount);
        }

        [Fact]
        public void LookupResultResponseItem_ShouldHaveCorrectValues()
        {
            // Arrange
            var item = new LookupResultResponseItem("test", "0 0");

            // Act
            // No action needed, just check values

            // Assert
            Assert.Equal("test", item.Word);
            Assert.Equal("0 0", item.Location);
        }

        [Fact]
        public void WordLocationEnum_ShouldHaveAllValues()
        {
            // Arrange
            // No action needed

            // Assert
            Assert.Equal(WordLocation.Dictionary, WordLocation.Dictionary);
            Assert.Equal(WordLocation.Merged, WordLocation.Merged);
            Assert.Equal(WordLocation.Included, WordLocation.Included);
            Assert.Equal(WordLocation.Excluded, WordLocation.Excluded);
            Assert.Equal(WordLocation.Error, WordLocation.Error);
        }
    }
}
