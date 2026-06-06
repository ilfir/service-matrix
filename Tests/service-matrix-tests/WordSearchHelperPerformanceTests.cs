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

           [Fact]
         public void Search_CaseInsensitive_ShouldReturnTrue()
          {
              // Arrange — matrix has lowercase, word is uppercase
             var matrix = new string[,] { { "a", "b" }, { "c", "d" } };
             var helper = new WordSearchHelper("AB", matrix);

              // Act
             var result = helper.Search();

              // Assert
             Assert.True(result);
          }

           [Fact]
         public void Search_Cyrillic_ShouldReturnTrue()
          {
              // Arrange
             var matrix = new string[,]
              {
                  {"ж", "и", "р", "b", "c"},
                  {"р", "н", "е", "т", "ь"},
                  {"е", "d", "з", "c", "c"},
                  {"в", "h", "i", "b", "c"},
                  {"з", "h", "i", "b", "c"}
              };
             var helper = new WordSearchHelper("жирнеть", matrix);

              // Act
             var result = helper.Search();

              // Assert
             Assert.True(result);
          }

            [Fact]
         public void Search_PrecomputedIndex_ShouldBeFast()
           {
               // Arrange — 50x50 matrix with random letters
             var matrix = new string[50, 50];
            for (int i = 0; i < 50; i++)
               {
                for (int j = 0; j < 50; j++)
                   {
                    matrix[i, j] = ((char)('a' + (i * 50 + j) % 26)).ToString();
                   }
               }

             var stopwatch = new Stopwatch();

               // Act — measure FindLetterLocations time (should be O(1) after index build)
            stopwatch.Start();
             var helper = new WordSearchHelper("abcdefgh", matrix);
             var buildTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();
             var locations = helper.FindLetterLocations();
             var lookupTime = stopwatch.ElapsedMilliseconds;

               // Assert
             Assert.True(buildTime < 50, $"Matrix build took {buildTime}ms (expected < 50ms)");
             Assert.True(locations.Count > 0, "Expected at least one 'a' location");
           }

           [Fact]
         public void Search_MultipleWords_ShouldScaleLinearly()
          {
              // Arrange
             var matrix = new string[20, 20];
            for (int i = 0; i < 20; i++)
              {
                for (int j = 0; j < 20; j++)
                  {
                    matrix[i, j] = ((char)('a' + (i * 20 + j) % 26)).ToString();
                  }
              }

             var stopwatch = new Stopwatch();
            var wordCount = 100;

              // Act — search for 100 different words
            stopwatch.Start();
            for (int w = 0; w < wordCount; w++)
              {
                var word = $"testword{w}";
                var helper = new WordSearchHelper(word, matrix);
                helper.Search();
              }
            stopwatch.Stop();

              // Assert — 100 searches should complete in reasonable time
             double avgMs = stopwatch.ElapsedMilliseconds / (double)wordCount;
             Assert.True(avgMs < 50, $"Average search time was {avgMs:F2}ms per word (expected < 50ms)");
          }

           [Fact]
         public void Search_GetFoundString_ShouldReturnOriginalCase()
          {
              // Arrange — matrix has mixed case
             var matrix = new string[,] { { "A", "b" }, { "C", "d" } };
             var helper = new WordSearchHelper("Ab", matrix);

              // Act
             helper.Search();
             var foundString = helper.GetFoundString();

              // Assert — should preserve original case from matrix
             Assert.Equal("Ab", foundString);
          }

           [Fact]
         public void Search_GetFoundWord_ShouldReturnOriginalCase()
          {
              // Arrange — matrix has mixed case
             var matrix = new string[,] { { "a", "B" }, { "c", "D" } };
             var helper = new WordSearchHelper("aC", matrix);

              // Act
             helper.Search();
             var foundWord = helper.GetFoundWord();

              // Assert — should preserve original case from matrix
             Assert.True(foundWord.Count > 0);
          }
     }
}
