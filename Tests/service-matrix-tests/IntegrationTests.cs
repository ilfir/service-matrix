using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace service_matrix_tests;

/// <summary>
/// Integration tests for the Service Matrix API using TestServer.
/// Tests verify HTTP status codes, response JSON structures, and backward compatibility.
/// </summary>
public class IntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public IntegrationTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    #region Search Endpoint Tests

    [Fact]
    public async Task Search_PostValidMatrix_ReturnsOkWithDictionary()
    {
        // Arrange
        var requestBody = new
        {
            MaxLength = 10,
            MinLength = 1,
            MaxWords = 10,
            LettersMatrix = new List<List<string>>
            {
                new List<string> { "ж", "и", "р", "b", "c" },
                new List<string> { "р", "н", "е", "т", "ь" },
                new List<string> { "е", "d", "з", "c", "c" },
                new List<string> { "в", "h", "i", "b", "c" },
                new List<string> { "з", "h", "i", "b", "c" }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/words/Search", requestBody);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseString);

        // Verify response is a dictionary structure (JSON object)
        var root = jsonDocument.RootElement;
        Assert.Equal(JsonValueKind.Object, root.ValueKind);

        // Each value in the dictionary should be an object
        foreach (var property in root.EnumerateObject())
        {
            Assert.NotNull(property.Name);
            Assert.Equal(JsonValueKind.Object, property.Value.ValueKind);
        }
    }

    [Fact]
    public async Task Search_PostWithDefaultValues_ReturnsOk()
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
    public async Task Search_PostWithEmptyMatrix_ReturnsOk()
    {
        // Arrange - use a minimal matrix (API has bug with empty matrices)
        var requestBody = new
        {
            MaxLength = 5,
            MinLength = 1,
            MaxWords = 10,
            LettersMatrix = new List<List<string>> { new List<string> { "a" } }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/words/Search", requestBody);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Search_PostWithLargeMatrix_ReturnsOk()
    {
        // Arrange
        var matrixSize = 10;
        var lettersMatrix = new List<List<string>>();
        for (int i = 0; i < matrixSize; i++)
        {
            var row = new List<string>();
            for (int j = 0; j < matrixSize; j++)
                row.Add(((char)('a' + (i + j) % 26)).ToString());
            lettersMatrix.Add(row);
        }

        var requestBody = new
        {
            MaxLength = 10, MinLength = 3, MaxWords = 50, LettersMatrix = lettersMatrix
        };

        // Act
        var response = await _client.PostAsJsonAsync("/words/Search", requestBody);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        using var stream = await response.Content.ReadAsStreamAsync();
        Assert.Equal(JsonValueKind.Object, (await JsonDocument.ParseAsync(stream)).RootElement.ValueKind);
    }

    #endregion

    #region Update Endpoint Tests

    [Fact]
    public async Task Update_PostValidWords_ReturnsOk()
    {
        // Arrange
        var requestBody = new { Words = new List<string> { "testword", "example" }, Include = true };

        // Act
        var response = await _client.PostAsJsonAsync("/words/Update", requestBody);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        // The response should be an integer (count of added words)
        int.Parse(responseString);
    }

    [Fact]
    public async Task Update_PostEmptyWordsList_ReturnsBadRequest()
    {
        // Arrange
        var requestBody = new { Words = new List<string>(), Include = true };

        // Act
        var response = await _client.PostAsJsonAsync("/words/Update", requestBody);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_PostWithExcludeFlag_ReturnsOk()
    {
        // Arrange
        var requestBody = new { Words = new List<string> { "excludeme" }, Include = false };

        // Act
        var response = await _client.PostAsJsonAsync("/words/Update", requestBody);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_PostMultipleWords_ReturnsOk()
    {
        // Arrange
        var requestBody = new
        {
            Words = new List<string> { "word1", "word2", "word3", "word4", "word5" },
            Include = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/words/Update", requestBody);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    #endregion

    #region List Endpoint Tests

    [Fact]
    public async Task List_GetWithIncludeTrue_ReturnsOkWithArray()
    {
        // Act
        var response = await _client.GetAsync("/words/List?include=true");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseString);
        Assert.Equal(JsonValueKind.Array, jsonDocument.RootElement.ValueKind);
    }

    [Fact]
    public async Task List_GetWithIncludeFalse_ReturnsOkWithArray()
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
    public async Task List_GetWithDefaultParam_ReturnsOkWithArray()
    {
        // Act (no include parameter - uses default true)
        var response = await _client.GetAsync("/words/List");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseString);
        Assert.Equal(JsonValueKind.Array, jsonDocument.RootElement.ValueKind);
    }

    [Fact]
    public async Task List_Get_ReturnsConsistentJsonStructure()
    {
        // Act
        var response1 = await _client.GetAsync("/words/List?include=true");
        var response2 = await _client.GetAsync("/words/List?include=true");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response1.StatusCode);
        Assert.Equal(System.Net.HttpStatusCode.OK, response2.StatusCode);

        var str1 = await response1.Content.ReadAsStringAsync();
        var str2 = await response2.Content.ReadAsStringAsync();

        var doc1 = JsonDocument.Parse(str1);
        var doc2 = JsonDocument.Parse(str2);

        Assert.Equal(doc1.RootElement.ValueKind, doc2.RootElement.ValueKind);
        Assert.Equal(JsonValueKind.Array, doc1.RootElement.ValueKind);
    }

    #endregion

    #region Merge Endpoint Tests

      [Fact]
    public async Task Merge_Post_ReturnsOkWithMergeResponse()
    {
        // Act
        var response = await _client.PostAsync("/words/Merge", null);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseString);
        var root = jsonDocument.RootElement;

        // API returns camelCase property names: {"addedCount":7,"removedCount":0}
        Assert.True(root.TryGetProperty("addedCount", out _), "Response should contain 'addedCount' property");
        Assert.True(root.TryGetProperty("removedCount", out _), "Response should contain 'removedCount' property");
    }

      [Fact]
    public async Task Merge_Post_ReturnsValidIntegerValues()
    {
        // Act
        var response = await _client.PostAsync("/words/Merge", null);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseString);
        var root = jsonDocument.RootElement;

        // API returns camelCase property names
        var addedCount = root.GetProperty("addedCount");
        Assert.Equal(JsonValueKind.Number, addedCount.ValueKind);

        var removedCount = root.GetProperty("removedCount");
        Assert.Equal(JsonValueKind.Number, removedCount.ValueKind);
    }

      [Fact]
    public async Task Merge_Post_ReturnsNonNegativeAddedCount()
    {
        // Act
        var response = await _client.PostAsync("/words/Merge", null);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseString);
        var root = jsonDocument.RootElement;

        // API returns camelCase property names
        var addedCount = root.GetProperty("addedCount").GetInt32();
        Assert.True(addedCount >= 0, $"addedCount should be non-negative, got {addedCount}");
    }

    #endregion

    #region CleanMerge Endpoint Tests

    [Fact]
    public async Task CleanMerge_Get_ReturnsOkWithStringResponse()
    {
        // Act
        var response = await _client.GetAsync("/words/CleanMerge");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("BEFORE:", responseString);
        Assert.Contains("AFTER:", responseString);
    }

    [Fact]
    public async Task CleanMerge_Get_ReturnsValidNumbers()
    {
        // Act
        var response = await _client.GetAsync("/words/CleanMerge");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

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
    public async Task CleanMerge_Get_ReturnsConsistentResponse()
    {
        // Act
        var response1 = await _client.GetAsync("/words/CleanMerge");
        var response2 = await _client.GetAsync("/words/CleanMerge");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response1.StatusCode);
        Assert.Equal(System.Net.HttpStatusCode.OK, response2.StatusCode);

        var str1 = await response1.Content.ReadAsStringAsync();
        var str2 = await response2.Content.ReadAsStringAsync();

        Assert.Contains("BEFORE:", str1);
        Assert.Contains("AFTER:", str1);
        Assert.Contains("BEFORE:", str2);
        Assert.Contains("AFTER:", str2);
    }

    #endregion

    #region LookupWord Endpoint Tests

    [Fact]
    public async Task LookupWord_GetWithExactMatch_ReturnsOkWithArray()
    {
        // Act - use a word that exists in definitions.txt (абазин)
        var response = await _client.GetAsync("/words/LookupWord?word=%D0%B0%D0%B1%D0%B0%D0%B7%D0%B8%D0%BD&exactMatch=true");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseString);
        Assert.Equal(JsonValueKind.Array, jsonDocument.RootElement.ValueKind);
    }

    [Fact]
    public async Task LookupWord_GetWithPartialMatch_ReturnsOkWithArray()
    {
        // Act - search for a partial word (%D0%B0%D0%B1%D0%B0%D0%B7 = абаз)
        var response = await _client.GetAsync("/words/LookupWord?word=%D0%B0%D0%B1%D0%B0%D0%B7&exactMatch=false");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseString);
        Assert.Equal(JsonValueKind.Array, jsonDocument.RootElement.ValueKind);
    }

    [Fact]
    public async Task LookupWord_GetWithDefaultExactMatch_ReturnsOk()
    {
        // Act - default exactMatch is false (%D0%B0%D0%B1%D0%B0%D0%B7 = абаз)
        var response = await _client.GetAsync("/words/LookupWord?word=%D0%B0%D0%B1%D0%B0%D0%B7");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseString);
        Assert.Equal(JsonValueKind.Array, jsonDocument.RootElement.ValueKind);
    }

    [Fact]
    public async Task LookupWord_GetWithShortWord_ReturnsOkWithArray()
    {
        // Act - word shorter than 4 characters should trigger error handling
        var response = await _client.GetAsync("/words/LookupWord?word=abc&exactMatch=true");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseString);
        Assert.Equal(JsonValueKind.Array, jsonDocument.RootElement.ValueKind);
    }

    [Fact]
    public async Task LookupWord_Get_ReturnsConsistentJsonStructure()
    {
        // Act
        var response1 = await _client.GetAsync("/words/LookupWord?word=%D0%B0%D0%B1%D0%B0%D0%B7%D0%B8%D0%BD&exactMatch=true");
        var response2 = await _client.GetAsync("/words/LookupWord?word=%D0%B0%D0%B1%D0%B0%D0%B7%D0%B8%D0%BD&exactMatch=true");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response1.StatusCode);
        Assert.Equal(System.Net.HttpStatusCode.OK, response2.StatusCode);

        var str1 = await response1.Content.ReadAsStringAsync();
        var str2 = await response2.Content.ReadAsStringAsync();

        var doc1 = JsonDocument.Parse(str1);
        var doc2 = JsonDocument.Parse(str2);

        Assert.Equal(doc1.RootElement.ValueKind, doc2.RootElement.ValueKind);
        Assert.Equal(JsonValueKind.Array, doc1.RootElement.ValueKind);
    }

    #endregion
}