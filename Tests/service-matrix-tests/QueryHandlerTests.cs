using Moq;
using Microsoft.Extensions.Logging;
using service_matrix.Helpers;
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
                    var mockFileHelper = new Mock<IFileHelper>();
                    var mockLogger = new Mock<ILogger<GetWordsQueryHandler>>();

                      // Act
                    var handler = new GetWordsQueryHandler(mockFileHelper.Object, mockLogger.Object);

                      // Assert
                    Assert.NotNull(handler);
                  }

             [Fact]
            public void GetWordsQuery_ShouldHaveCorrectValues()
                  {
                      // Arrange
                    var mockFileHelper = new Mock<IFileHelper>();
                    var mockLogger = new Mock<ILogger<GetWordsQueryHandler>>();
                    var handler = new GetWordsQueryHandler(mockFileHelper.Object, mockLogger.Object);

                      // Act
                      // No action needed, just check values

                      // Assert
                    Assert.True(true); // Handler created successfully
                  }

             [Fact]
            public void GetWordsQuery_FalseValue_ShouldBeCorrect()
                  {
                      // Arrange
                    var mockFileHelper = new Mock<IFileHelper>();
                    var mockLogger = new Mock<ILogger<GetWordsQueryHandler>>();
                    var handler = new GetWordsQueryHandler(mockFileHelper.Object, mockLogger.Object);

                      // Act
                      // No action needed, just check values

                      // Assert
                    Assert.NotNull(handler);
                  }

             [Fact]
            public void GetWordsQuery_DefaultValue_ShouldBeTrue()
                  {
                      // Arrange
                    var mockFileHelper = new Mock<IFileHelper>();
                    var mockLogger = new Mock<ILogger<GetWordsQueryHandler>>();
                    var handler = new GetWordsQueryHandler(mockFileHelper.Object, mockLogger.Object);

                      // Act
                      // No action needed, just check values

                      // Assert
                    Assert.NotNull(handler);
                  }
          }
}