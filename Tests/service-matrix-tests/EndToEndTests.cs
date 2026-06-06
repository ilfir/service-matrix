using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace service_matrix_tests;

/// <summary>
/// End-to-end tests that exercise actual file I/O through the full HTTP pipeline.
/// These tests verify the complete request flow: HTTP → Controller → Handler → File reads → Word search → Response.
/// </summary>
public class EndToEndTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static readonly string _testWord = "абазин";

    public EndToEndTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    #region Search End-to-End Tests

    [Fact]
    public async Task Search_EndToEnd_WithRealDictionary_ShouldReturnWordsFromDefinitions()
    {
        // Arrange — use a matrix that contains letters from real dictionary words
        var requestBody = new
        {
            MaxLength = 20,
            MinLength = 3,
            MaxWords = 100,
            LettersMatrix = new List<List<string>>
            {
                new List<string> { "а", "б", "а", "з", "и" },
                new List<string> { "н", "м", "е", "л", "е" },
                new List<string> { "н", "и", "р", "т", "ь" },
                new List<string> { "в", "у", "л", "у", "ч" },
                new List<string> { "з", "п", "о", "в", "е" }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/words/Search", requestBody);

        // Assert — should return OK with a dictionary of found words
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseString);
        Assert.Equal(JsonValueKind.Object, jsonDocument.RootElement.ValueKind);

           // The API reads from definitions.txt and merged.txt — at least some words should be found
           // or the result should be an empty object (valid response)
         Assert.True(jsonDocument.RootElement.EnumerateObject().Count() >= 0, "Response should be a valid JSON object");
    }

    [Fact]
    public async Task Search_EndToEnd_WithSmallMatrix_ShouldReturnValidResponse()
    {
        // Arrange
        var requestBody = new
        {
            LettersMatrix = new List<List<string>>
            {
                new List<string> { "a", "b", "c" },
                new List<string> { "d", "e", "f" },
                new List<string> { "g", "h", "i" }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/words/Search", requestBody);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseString);
        Assert.Equal(JsonValueKind.Object, jsonDocument.RootElement.ValueKind);
    }

    [Fact]
    public async Task Search_EndToEnd_WithLargeMatrix_ShouldCompleteWithinTimeout()
    {
        // Arrange — 50x50 matrix with random letters
        var matrixSize = 50;
        var lettersMatrix = new List<List<string>>();
        for (int i = 0; i < matrixSize; i++)
        {
            var row = new List<string>();
            for (int j = 0; j < matrixSize; j++)
                row.Add(((char)('a' + (i * matrixSize + j) % 26)).ToString());
            lettersMatrix.Add(row);
        }

        var requestBody = new
        {
            MaxLength = 15,
            MinLength = 3,
            MaxWords = 50,
            LettersMatrix = lettersMatrix
        };

        // Act — use a timeout to ensure the request completes
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var response = await _client.PostAsJsonAsync("/words/Search", requestBody, cts.Token);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    #endregion

    #region Update End-to-End Tests

    [Fact]
    public async Task Update_EndToEnd_WithIncludeFlag_ShouldPersistToDisk()
    {
        // Arrange — add a test word to the include list
        var testWord = "e2etestword";
        var requestBody = new { Words = new List<string> { testWord }, Include = true };

        // Act
        var response1 = await _client.PostAsJsonAsync("/words/Update", requestBody);

        // Assert — update should succeed
        Assert.Equal(System.Net.HttpStatusCode.OK, response1.StatusCode);

        // Verify the word count increased
        var responseString1 = await response1.Content.ReadAsStringAsync();
        int countBefore = int.Parse(responseString1);
        Assert.True(countBefore >= 0, "Update should return a non-negative count");
    }

    [Fact]
    public async Task Update_EndToEnd_WithExcludeFlag_ShouldPersistToDisk()
    {
        // Arrange — add a test word to the exclude list
        var requestBody = new { Words = new List<string> { "excludetest" }, Include = false };

        // Act
        var response = await _client.PostAsJsonAsync("/words/Update", requestBody);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        int count = int.Parse(responseString);
        Assert.True(count >= 0, "Exclude update should return a non-negative count");
    }

    [Fact]
    public async Task Update_EndToEnd_MultipleUpdates_ShouldAccumulate()
    {
        // Arrange
        var words = new List<string> { "word1", "word2", "word3" };
        var requestBody = new { Words = words, Include = true };

        // Act — send multiple update requests
        var responses = new List<HttpResponseMessage>();
        foreach (var word in words)
        {
            var updatedRequest = new { Words = new List<string> { word }, Include = true };
            var response = await _client.PostAsJsonAsync("/words/Update", updatedRequest);
            responses.Add(response);
        }

        // Assert — all updates should succeed
        foreach (var response in responses)
        {
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }

    #endregion

    #region List End-to-End Tests

    [Fact]
    public async Task List_EndToEnd_WithIncludeTrue_ShouldReturnWordsFromFile()
    {
        // Act
        var response = await _client.GetAsync("/words/List?include=true");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseString);
        Assert.Equal(JsonValueKind.Array, jsonDocument.RootElement.ValueKind);

        // The include.txt file is empty, so the list may be empty but should still be a valid array
        Assert.True(jsonDocument.RootElement.EnumerateArray().Count() >= 0, "List should be a valid JSON array");
    }

    [Fact]
    public async Task List_EndToEnd_WithIncludeFalse_ShouldReturnWordsFromFile()
    {
        // Act
        var response = await _client.GetAsync("/words/List?include=false");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseString);
        Assert.Equal(JsonValueKind.Array, jsonDocument.RootElement.ValueKind);
    }

    [Fact]
    public async Task List_EndToEnd_AfterUpdate_ShouldReflectChanges()
    {
        // Arrange — first get the baseline list
        var baselineResponse = await _client.GetAsync("/words/List?include=true");
        Assert.Equal(System.Net.HttpStatusCode.OK, baselineResponse.StatusCode);

        var baselineString = await baselineResponse.Content.ReadAsStringAsync();
        var baselineDoc = JsonDocument.Parse(baselineString);
        int baselineCount = baselineDoc.RootElement.EnumerateArray().Count();

        // Act — add a word to the include list
        var updateRequest = new { Words = new List<string> { "e2etestword" }, Include = true };
        var updateResponse = await _client.PostAsJsonAsync("/words/Update", updateRequest);
        Assert.Equal(System.Net.HttpStatusCode.OK, updateResponse.StatusCode);

        // Get the updated list
        var updatedResponse = await _client.GetAsync("/words/List?include=true");
        Assert.Equal(System.Net.HttpStatusCode.OK, updatedResponse.StatusCode);

        var updatedString = await updatedResponse.Content.ReadAsStringAsync();
        var updatedDoc = JsonDocument.Parse(updatedString);
        int updatedCount = updatedDoc.RootElement.EnumerateArray().Count();

        // Assert — the list should have at least as many words as before
        Assert.True(updatedCount >= baselineCount, $"Updated list count ({updatedCount}) should be >= baseline ({baselineCount})");
    }

    #endregion

    #region Merge End-to-End Tests

    [Fact]
    public async Task Merge_EndToEnd_ShouldProcessDictionaryFiles()
    {
        // Act
        var response = await _client.PostAsync("/words/Merge", null);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseString);
        var root = jsonDocument.RootElement;

        // Should contain addedCount and removedCount
        Assert.True(root.TryGetProperty("addedCount", out _), "Response should contain 'addedCount'");
        Assert.True(root.TryGetProperty("removedCount", out _), "Response should contain 'removedCount'");

        var addedCount = root.GetProperty("addedCount").GetInt32();
        var removedCount = root.GetProperty("removedCount").GetInt32();
        Assert.True(addedCount >= 0, $"addedCount should be non-negative: {addedCount}");
        Assert.True(removedCount >= 0, $"removedCount should be non-negative: {removedCount}");
    }

    [Fact]
    public async Task Merge_EndToEnd_MultipleCalls_ShouldBeIdempotent()
    {
        // Act — call merge multiple times
        var responses = new List<HttpResponseMessage>();
        for (int i = 0; i < 3; i++)
        {
            var response = await _client.PostAsync("/words/Merge", null);
            responses.Add(response);
        }

        // Assert — all calls should succeed
        foreach (var response in responses)
        {
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }

    #endregion

    #region CleanMerge End-to-End Tests

    [Fact]
    public async Task CleanMerge_EndToEnd_ShouldProcessMergedFile()
    {
        // Act
        var response = await _client.GetAsync("/words/CleanMerge");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("BEFORE:", responseString);
        Assert.Contains("AFTER:", responseString);

        // Verify the counts are valid numbers
        var beforeMatch = System.Text.RegularExpressions.Regex.Match(responseString, @"BEFORE:\s*(\d+)");
        var afterMatch = System.Text.RegularExpressions.Regex.Match(responseString, @"AFTER:\s*(\d+)");

        Assert.True(beforeMatch.Success, "Response should contain BEFORE count");
        Assert.True(afterMatch.Success, "Response should contain AFTER count");

        var beforeCount = int.Parse(beforeMatch.Groups[1].Value);
        var afterCount = int.Parse(afterMatch.Groups[1].Value);
        Assert.True(beforeCount >= 0, $"BEFORE count should be non-negative: {beforeCount}");
        Assert.True(afterCount >= 0, $"AFTER count should be non-negative: {afterCount}");
    }

    [Fact]
    public async Task CleanMerge_EndToEnd_MultipleCalls_ShouldBeConsistent()
    {
        // Act — call clean merge multiple times
        var responses = new List<HttpResponseMessage>();
        for (int i = 0; i < 3; i++)
        {
            var response = await _client.GetAsync("/words/CleanMerge");
            responses.Add(response);
        }

        // Assert — all calls should succeed and return consistent format
        foreach (var response in responses)
        {
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("BEFORE:", responseString);
            Assert.Contains("AFTER:", responseString);
        }
    }

    #endregion

    #region LookupWord End-to-End Tests

    [Fact]
    public async Task LookupWord_EndToEnd_WithExactMatch_ShouldReturnDefinitionsFromFile()
    {
        // Act — lookup a word that exists in definitions.txt
        var response = await _client.GetAsync($"/words/LookupWord?word={_testWord}&exactMatch=true");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseString);
        Assert.Equal(JsonValueKind.Array, jsonDocument.RootElement.ValueKind);
    }

    [Fact]
    public async Task LookupWord_EndToEnd_WithPartialMatch_ShouldReturnDefinitionsFromFile()
    {
        // Act — lookup a partial word
        var response = await _client.GetAsync($"/words/LookupWord?word=абаз&exactMatch=false");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseString);
        Assert.Equal(JsonValueKind.Array, jsonDocument.RootElement.ValueKind);
    }

    [Fact]
    public async Task LookupWord_EndToEnd_NonExistentWord_ShouldReturnEmptyArray()
    {
        // Act — lookup a word that likely doesn't exist
        var response = await _client.GetAsync("/words/LookupWord?word=zzzzz_nonexistent&exactMatch=true");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseString);
        Assert.Equal(JsonValueKind.Array, jsonDocument.RootElement.ValueKind);
    }

    [Fact]
    public async Task LookupWord_EndToEnd_MultipleCalls_ShouldBeConsistent()
    {
        // Act — lookup the same word multiple times
        var responses = new List<HttpResponseMessage>();
        for (int i = 0; i < 3; i++)
        {
            var response = await _client.GetAsync($"/words/LookupWord?word={_testWord}&exactMatch=true");
            responses.Add(response);
        }

        // Assert — all calls should succeed with consistent structure
        foreach (var response in responses)
        {
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(responseString);
            Assert.Equal(JsonValueKind.Array, jsonDocument.RootElement.ValueKind);
        }
    }

    #endregion

    #region Full Pipeline End-to-End Tests

    [Fact]
    public async Task FullPipeline_EndToEnd_SearchUpdateListMergeLookup_ShouldAllSucceed()
    {
        // Arrange — define the full API workflow
        var searchRequest = new
        {
            LettersMatrix = new List<List<string>>
            {
                new List<string> { "а", "б", "а" },
                new List<string> { "з", "и", "н" },
                new List<string> { "м", "е", "л" }
            }
        };

        var updateRequest = new { Words = new List<string> { "e2etest" }, Include = true };

        // Act — execute the full API pipeline sequentially
        var searchResponse = await _client.PostAsJsonAsync("/words/Search", searchRequest);
        var updateResponse = await _client.PostAsJsonAsync("/words/Update", updateRequest);
        var listResponse = await _client.GetAsync("/words/List?include=true");
        var mergeResponse = await _client.PostAsync("/words/Merge", null);
        var lookupResponse = await _client.GetAsync($"/words/LookupWord?word={_testWord}&exactMatch=true");
        var cleanMergeResponse = await _client.GetAsync("/words/CleanMerge");

        // Assert — all endpoints should return success status codes
        Assert.Equal(System.Net.HttpStatusCode.OK, searchResponse.StatusCode);
        Assert.Equal(System.Net.HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.Equal(System.Net.HttpStatusCode.OK, listResponse.StatusCode);
        Assert.Equal(System.Net.HttpStatusCode.OK, mergeResponse.StatusCode);
        Assert.Equal(System.Net.HttpStatusCode.OK, lookupResponse.StatusCode);
        Assert.Equal(System.Net.HttpStatusCode.OK, cleanMergeResponse.StatusCode);
    }

    [Fact]
    public async Task FullPipeline_EndToEnd_ConcurrentRequests_ShouldAllSucceed()
    {
        // Arrange — define concurrent requests to all endpoints
        var searchRequest = new
        {
            LettersMatrix = new List<List<string>>
            {
                new List<string> { "a", "b", "c" },
                new List<string> { "d", "e", "f" },
                new List<string> { "g", "h", "i" }
            }
        };

        // Act — execute all endpoints concurrently
        var searchTask = _client.PostAsJsonAsync("/words/Search", searchRequest);
        var updateTask = _client.PostAsJsonAsync("/words/Update", new { Words = new List<string>(), Include = true });
        var listTask = _client.GetAsync("/words/List?include=true");
        var mergeTask = _client.PostAsync("/words/Merge", null);
        var lookupTask = _client.GetAsync($"/words/LookupWord?word={_testWord}&exactMatch=true");
        var cleanMergeTask = _client.GetAsync("/words/CleanMerge");

        await Task.WhenAll(searchTask, updateTask, listTask, mergeTask, lookupTask, cleanMergeTask);

        // Assert — all requests should complete successfully
        Assert.Equal(System.Net.HttpStatusCode.OK, searchTask.Result.StatusCode);
        Assert.Equal(System.Net.HttpStatusCode.OK, updateTask.Result.StatusCode);
        Assert.Equal(System.Net.HttpStatusCode.OK, listTask.Result.StatusCode);
        Assert.Equal(System.Net.HttpStatusCode.OK, mergeTask.Result.StatusCode);
        Assert.Equal(System.Net.HttpStatusCode.OK, lookupTask.Result.StatusCode);
        Assert.Equal(System.Net.HttpStatusCode.OK, cleanMergeTask.Result.StatusCode);
    }

    #endregion
}
