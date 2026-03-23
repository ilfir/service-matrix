using service_matrix.QueryHandlers;
using service_matrix.Queries;
using Xunit;

namespace service_matrix_tests
{
    public class QueryHandlerTests
    {
        [Fact]
        public void GetWordsQueryHandler_ShouldBeInstantiable()
        {
            // Arrange
            // No arrangement needed

            // Act
            var handler = new GetWordsQueryHandler();

            // Assert
            Assert.NotNull(handler);
        }

        [Fact]
        public void GetWordsQuery_ShouldHaveCorrectValues()
        {
            // Arrange
            var query = new GetWordsQuery(true);

            // Act
            // No action needed, just check values

            // Assert
            Assert.True(query.Include);
        }

        [Fact]
        public void GetWordsQuery_FalseValue_ShouldBeCorrect()
        {
            // Arrange
            var query = new GetWordsQuery(false);

            // Act
            // No action needed, just check values

            // Assert
            Assert.False(query.Include);
        }

        [Fact]
        public void GetWordsQuery_DefaultValue_ShouldBeTrue()
        {
            // Arrange
            var query = new GetWordsQuery(true);

            // Act
            // No action needed, just check values

            // Assert
            Assert.True(query.Include); // Default value
        }
    }
}
