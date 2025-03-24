// using service_matrix.Helpers;
// using Xunit;
//
// namespace service_matrix.Tests.Helpers
// {
//     public class WordSearchHelperTests
//     {
//         [Fact]
//         public void CopyArray_ShouldReturnExactCopy()
//         {
//             // Arrange
//             var word = "test";
//             string[,] source = {
//                 { "a", "b", "c", "b", "c" },
//                 { "d", "t", "f", "b", "c" },
//                 { "g", "h", "s", "b", "c" },
//                 { "g", "h", "i", "e", "c" },
//                 { "g", "h", "i", "b", "t" }
//             };
//             var helper = new WordSearchHelper("word", source);
//
//             // Act
//             var result = helper.CopyArray(source);
//
//             // Assert
//             Assert.Equal(source, result);
//             Assert.True(helper.Search());
//             Assert.Equal(word, helper.GetFoundString());
//         }
//
//         [Fact]
//         public void IsAllLettersInMatrix_ShouldReturnTrue_WhenAllLettersArePresent()
//         {
//             // Arrange
//             string word = "doc";
//             string[,] source = {
//                 { "a", "b", "c", "b", "c" },
//                 { "d", "e", "f", "b", "c" },
//                 { "g", "d", "o", "c", "c" },
//                 { "g", "h", "i", "b", "c" },
//                 { "g", "h", "i", "b", "c" }
//             };
//             var helper = new WordSearchHelper(word, source);
//
//             // Act
//             var result = helper.IsAllLettersInMatrix(source, word);
//
//             // Assert
//             Assert.True(result);
//         }
//
//         [Fact]
//         public void IsAllLettersInMatrix_ShouldReturnFalse_WhenNotAllLettersArePresent()
//         {
//             // Arrange
//             string word = "xyz";
//             string[,] source = {
//                 { "a", "b", "c", "b", "c" },
//                 { "d", "e", "f", "b", "c" },
//                 { "g", "d", "o", "c", "c" },
//                 { "g", "h", "i", "b", "c" },
//                 { "g", "h", "i", "b", "c" }
//             };
//             var helper = new WordSearchHelper(word, source);
//
//
//             // Act
//             var result = helper.IsAllLettersInMatrix(source, word);
//
//             // Assert
//             Assert.False(result);
//         }
//
//         [Fact]
//         public void IsNeighborToNextLetter_ShouldReturnTrue_WhenNextLetterIsNeighbor()
//         {
//             // Arrange
//             string[,] source = {
//                 { "a", "b", "c", "b", "c" },
//                 { "d", "e", "f", "b", "c" },
//                 { "g", "d", "o", "c", "c" },
//                 { "g", "h", "i", "b", "c" },
//                 { "g", "h", "i", "b", "c" }
//             };
//             var helper = new WordSearchHelper("ab", source);
//             string[] word = { "a", "b" };
//
//             // Act
//             var result = helper.IsNeighborToNextLetter(0, 0, word, 0, source);
//
//             // Assert
//             Assert.True(result);
//         }
//
//         [Fact]
//         public void IsNeighborToNextLetter_ShouldReturnFalse_WhenNextLetterIsNotNeighbor()
//         {
//             // Arrange
//             string[,] source = {
//                 { "a", "b", "c", "b", "c" },
//                 { "d", "e", "f", "b", "c" },
//                 { "g", "d", "o", "c", "c" },
//                 { "g", "h", "i", "b", "c" },
//                 { "g", "h", "i", "b", "c" }
//             };
//             var helper = new WordSearchHelper("ai", source);
//             string[] word = { "a", "i" };
//
//             // Act
//             var result = helper.IsNeighborToNextLetter(0, 0, word, 0, source);
//
//             // Assert
//             Assert.False(result);
//         }
//     }
// }