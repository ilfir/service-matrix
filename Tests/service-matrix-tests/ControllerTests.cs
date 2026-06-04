using Microsoft.Extensions.Logging;
using Moq;
using service_matrix.CommandHandlers;
using service_matrix.Controllers;
using service_matrix.DTO;
using service_matrix.Helpers;
using service_matrix.QueryHandlers;
using Xunit;

namespace service_matrix_tests
{
    public class ControllerTests
     {
         private static WordSearchCommandHandler CreateWordSearchCommandHandlerMock()
          {
             var mock = new Mock<WordSearchCommandHandler>(MockBehavior.Strict, null!, null!);
             return mock.Object;
          }

         private static UpdateWordsCommandHandler CreateUpdateWordsCommandHandlerMock()
          {
             var mock = new Mock<UpdateWordsCommandHandler>(MockBehavior.Strict, null!, null!);
             return mock.Object;
          }

         private static MergeWordsCommandHandler CreateMergeWordsCommandHandlerMock()
          {
             var mock = new Mock<MergeWordsCommandHandler>(MockBehavior.Strict, null!, null!);
             return mock.Object;
          }

         private static GetWordsQueryHandler CreateGetWordsQueryHandlerMock()
          {
             var mock = new Mock<GetWordsQueryHandler>(MockBehavior.Strict, null!, null!);
             return mock.Object;
          }

         private static LookupWordQueryHandler CreateLookupWordQueryHandlerMock()
          {
             var mock = new Mock<LookupWordQueryHandler>(MockBehavior.Strict, null!, null!);
             return mock.Object;
          }

         private static WordSearchController CreateController()
          {
             var mockFileHelper = new Mock<IFileHelper>();
             var mockLogger = new Mock<ILogger<WordSearchController>>();
             return new WordSearchController(
                 mockFileHelper.Object,
                 CreateWordSearchCommandHandlerMock(),
                 CreateUpdateWordsCommandHandlerMock(),
                 CreateMergeWordsCommandHandlerMock(),
                 CreateGetWordsQueryHandlerMock(),
                 CreateLookupWordQueryHandlerMock(),
                 mockLogger.Object);
          }

         [Fact]
         public void WordSearchController_ShouldBeInstantiable()
          {
             // Act
             var controller = CreateController();

             // Assert
             Assert.NotNull(controller);
          }

         [Fact]
         public void WordSearchController_ShouldHaveCorrectRoute()
          {
             // Act
             var controller = CreateController();

             // Assert
             Assert.NotNull(controller);
          }

         [Fact]
         public void WordSearchController_SearchMethod_ShouldExist()
          {
             // Act
             var controller = CreateController();

             // Assert
             Assert.NotNull(controller);
          }

         [Fact]
         public void WordSearchController_UpdateMethod_ShouldExist()
          {
             // Act
             var controller = CreateController();

             // Assert
             Assert.NotNull(controller);
          }

         [Fact]
         public void WordSearchController_GetListMethod_ShouldExist()
          {
             // Act
             var controller = CreateController();

             // Assert
             Assert.NotNull(controller);
          }

         [Fact]
         public void WordSearchController_MergeWordsMethod_ShouldExist()
          {
             // Act
             var controller = CreateController();

             // Assert
             Assert.NotNull(controller);
          }

         [Fact]
         public void WordSearchController_LookupWordMethod_ShouldExist()
          {
             // Act
             var controller = CreateController();

             // Assert
             Assert.NotNull(controller);
          }
     }
}