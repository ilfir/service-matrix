using service_matrix.Helpers;
using Xunit;

namespace service_matrix_tests
{
    public class FileHelperTests
    {
        [Fact]
        public async Task ReadFileAsync_ExistingFile_ShouldReturnLines()
        {
            // Arrange
            var tempFile = Path.Combine(AppContext.BaseDirectory, "test", "temp.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(tempFile));
            File.WriteAllLines(tempFile, new[] { "line1", "line2", "line3" });

            // Act
            var helper = new FileHelper(null);
             var result = await helper.ReadFileAsync("test", "temp.txt");
             var resultList = result.ToList();

            // Assert
            Assert.Equal(3, resultList.Count);
            Assert.Equal("line1", resultList[0]);
            Assert.Equal("line2", resultList[1]);
            Assert.Equal("line3", resultList[2]);

            // Cleanup
            File.Delete(tempFile);
        }

        [Fact]
        public async Task ReadFileAsync_NonExistingFile_ShouldReturnEmpty()
        {
            // Arrange
            var tempFile = Path.Combine(AppContext.BaseDirectory, "test", "nonexistent.txt");

            // Act
            var helper2 = new FileHelper(null);
             var resultList2 = (await helper2.ReadFileAsync("test", "nonexistent.txt")).ToList();

            // Assert
            Assert.Empty(resultList2);
        }

        [Fact]
        public async Task WriteFileNewContents_ShouldCreateFile()
        {
            // Arrange
            var tempFile = Path.Combine(AppContext.BaseDirectory, "test", "new.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(tempFile));

            // Act
            await FileHelper.WriteFileNewContents(new[] { "content1", "content2" }, "test", "new.txt");

            // Assert
            Assert.True(File.Exists(tempFile));
            var lines = File.ReadAllLines(tempFile).ToList();
            Assert.Equal(2, lines.Count);
            Assert.Equal("content1", lines[0]);
            Assert.Equal("content2", lines[1]);

            // Cleanup
            File.Delete(tempFile);
        }

        [Fact]
        public async Task WriteFileAppend_ShouldAppendToExistingFile()
        {
            // Arrange
            var tempFile = Path.Combine(AppContext.BaseDirectory, "test", "append.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(tempFile));
            File.WriteAllLines(tempFile, new[] { "existing1" });

            // Act
            await FileHelper.WriteFileAppend(new[] { "new1", "new2" }, "test", "append.txt");

            // Assert
            var lines = File.ReadAllLines(tempFile).ToList();
            Assert.Equal(3, lines.Count);
            Assert.Equal("existing1", lines[0]);
            Assert.Equal("new1", lines[1]);
            Assert.Equal("new2", lines[2]);

            // Cleanup
            File.Delete(tempFile);
        }

        [Fact]
        public async Task ReadFileAsync_EmptyFile_ShouldReturnEmpty()
        {
            // Arrange
            var tempFile = Path.Combine(AppContext.BaseDirectory, "test", "empty.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(tempFile));
            File.WriteAllText(tempFile, "");

            // Act
            var helper3 = new FileHelper(null);
             var resultList3 = (await helper3.ReadFileAsync("test", "empty.txt")).ToList();

            // Assert
            Assert.Empty(resultList3);

            // Cleanup
            File.Delete(tempFile);
        }
    }
}
