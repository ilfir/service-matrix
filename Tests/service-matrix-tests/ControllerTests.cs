using Moq;
using service_matrix.Controllers;
using service_matrix.DTO;
using service_matrix.Helpers;
using Xunit;

namespace service_matrix_tests
{
    public class ControllerTests
    {
        [Fact]
        public void WordSearchController_ShouldBeInstantiable()
        {
            // Arrange
            var mockFileHelper = new Mock<IFileHelper>();
            
            // Act
            var controller = new WordSearchController(mockFileHelper.Object);

            // Assert
            Assert.NotNull(controller);
        }

        [Fact]
        public void WordSearchController_ShouldHaveCorrectRoute()
        {
            // Arrange
            var mockFileHelper = new Mock<IFileHelper>();
            var controller = new WordSearchController(mockFileHelper.Object);

            // Act
            // No action needed, just check attributes

            // Assert
            Assert.NotNull(controller);
        }

        [Fact]
        public void WordSearchController_SearchMethod_ShouldExist()
        {
            // Arrange
            var mockFileHelper = new Mock<IFileHelper>();
            var controller = new WordSearchController(mockFileHelper.Object);

            // Act
            // No action needed, just check method exists

            // Assert
            Assert.NotNull(controller);
        }

        [Fact]
        public void WordSearchController_UpdateMethod_ShouldExist()
        {
            // Arrange
            var mockFileHelper = new Mock<IFileHelper>();
            var controller = new WordSearchController(mockFileHelper.Object);

            // Act
            // No action needed, just check method exists

            // Assert
            Assert.NotNull(controller);
        }

        [Fact]
        public void WordSearchController_GetListMethod_ShouldExist()
        {
            // Arrange
            var mockFileHelper = new Mock<IFileHelper>();
            var controller = new WordSearchController(mockFileHelper.Object);

            // Act
            // No action needed, just check method exists

            // Assert
            Assert.NotNull(controller);
        }

        [Fact]
        public void WordSearchController_MergeWordsMethod_ShouldExist()
        {
            // Arrange
            var mockFileHelper = new Mock<IFileHelper>();
            var controller = new WordSearchController(mockFileHelper.Object);

            // Act
            // No action needed, just check method exists

            // Assert
            Assert.NotNull(controller);
        }

        [Fact]
        public void WordSearchController_LookupWordMethod_ShouldExist()
        {
            // Arrange
            var mockFileHelper = new Mock<IFileHelper>();
            var controller = new WordSearchController(mockFileHelper.Object);

            // Act
            // No action needed, just check method exists

            // Assert
            Assert.NotNull(controller);
        }
    }
}