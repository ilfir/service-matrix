using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Net.Http.Json;
using Xunit;

namespace service_matrix_tests;

/// <summary>
/// Tests for the exception handling middleware.
/// </summary>
public class MiddlewareTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MiddlewareTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetList_ReturnsOkAndValidJson()
    {
        // Act
        var response = await _client.GetAsync("/words/List?include=true");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var jsonDocument = System.Text.Json.JsonDocument.Parse(responseString);
        Assert.Equal(System.Text.Json.JsonValueKind.Array, jsonDocument.RootElement.ValueKind);
    }

    [Fact]
    public async Task Search_PostValidMatrix_ReturnsOk()
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
    }

    [Fact]
    public async Task Update_PostValidWords_ReturnsOk()
    {
        // Arrange
        var requestBody = new { Words = new List<string> { "testword" }, Include = true };

        // Act
        var response = await _client.PostAsJsonAsync("/words/Update", requestBody);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Merge_Post_ReturnsOk()
    {
        // Act
        var response = await _client.PostAsync("/words/Merge", null);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CleanMerge_Get_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/words/CleanMerge");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task LookupWord_GetWithExactMatch_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/words/LookupWord?word=abc&exactMatch=true");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}