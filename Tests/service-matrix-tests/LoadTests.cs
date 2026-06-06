using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using service_matrix.Helpers;
using service_matrix_tests;
using Xunit;

namespace service_matrix_load_tests;

/// <summary>
/// Load and Performance Tests - Verify API behavior under repeated conditions.
/// </summary>
public class LoadTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static readonly string _testWord = "абазин";

    public LoadTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    #region Repeated Request Performance Tests

    [Fact]
    public void Search_RepeatedRequests_ShouldCompleteWithinTimeLimit()
    {
        var requestBody = new
        {
            MaxLength = 5, MinLength = 1, MaxWords = 10,
            LettersMatrix = new List<List<string>>
            {
                new List<string> { "a", "b", "c" },
                new List<string> { "d", "e", "f" },
                new List<string> { "g", "h", "i" }
            }
        };

        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < 50; i++)
            _ = _client.PostAsJsonAsync("/words/Search", requestBody).Result;
        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds < 30000,
            $"50 search requests took {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void Update_RepeatedRequests_ShouldCompleteWithinTimeLimit()
    {
        var requestBody = new { Words = new List<string> { "testword" }, Include = true };

        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < 100; i++)
            _ = _client.PostAsJsonAsync("/words/Update", requestBody).Result;
        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds < 30000,
            $"100 update requests took {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void List_RepeatedRequests_ShouldCompleteWithinTimeLimit()
    {
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < 100; i++)
            _ = _client.GetAsync("/words/List?include=true").Result;
        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds < 30000,
            $"100 list requests took {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void Merge_RepeatedRequests_ShouldCompleteWithinTimeLimit()
    {
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < 50; i++)
            _ = _client.PostAsync("/words/Merge", null).Result;
        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds < 30000,
            $"50 merge requests took {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void LookupWord_RepeatedRequests_ShouldCompleteWithinTimeLimit()
    {
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < 100; i++)
            _ = _client.GetAsync($"/words/LookupWord?word={_testWord}&exactMatch=true").Result;
        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds < 30000,
            $"100 lookup requests took {stopwatch.ElapsedMilliseconds}ms");
    }

    #endregion

    #region Concurrent Request Tests

    [Fact]
    public void Search_ConcurrentRequests_ShouldAllSucceed()
    {
        var requestBody = new
        {
            LettersMatrix = new List<List<string>>
            {
                new List<string> { "a", "b", "c" },
                new List<string> { "d", "e", "f" },
                new List<string> { "g", "h", "i" }
            }
        };

        var tasks = new List<Task<HttpStatusCode>>();
        for (int i = 0; i < 20; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                var response = await _client.PostAsJsonAsync("/words/Search", requestBody);
                return response.StatusCode;
            }));
        }

        var results = Task.WhenAll(tasks).Result;
        foreach (var statusCode in results)
            Assert.Equal(HttpStatusCode.OK, statusCode);
    }

    [Fact]
    public void List_ConcurrentRequests_ShouldAllSucceed()
    {
        var tasks = new List<Task<HttpStatusCode>>();
        for (int i = 0; i < 50; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                var response = await _client.GetAsync("/words/List?include=true");
                return response.StatusCode;
            }));
        }

        var results = Task.WhenAll(tasks).Result;
        foreach (var statusCode in results)
            Assert.Equal(HttpStatusCode.OK, statusCode);
    }

    #endregion

    #region Stress Tests

    [Fact]
    public void FullApiCycle_StressTest_ShouldComplete()
    {
        var searchRequest = new
        {
            LettersMatrix = new List<List<string>>
            {
                new List<string> { "a", "b", "c" },
                new List<string> { "d", "e", "f" },
                new List<string> { "g", "h", "i" }
            }
        };

        var stopwatch = Stopwatch.StartNew();
        _ = _client.PostAsJsonAsync("/words/Search", searchRequest).Result;
        _ = _client.PostAsJsonAsync("/words/Update", new { Words = new List<string>(), Include = true }).Result;
        _ = _client.GetAsync("/words/List?include=true").Result;
        _ = _client.GetAsync($"/words/LookupWord?word={_testWord}&exactMatch=true").Result;
        _ = _client.PostAsync("/words/Merge", null).Result;
        _ = _client.GetAsync("/words/CleanMerge").Result;
        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds < 30000,
            $"Full API cycle took {stopwatch.ElapsedMilliseconds}ms");
    }

    #endregion

    #region WordSearchHelper Performance Tests

    [Fact]
    public void WordSearchHelper_Search_LargeMatrix_ShouldCompleteWithinTimeLimit()
    {
        var matrixSize = 50;
        var source = new string[matrixSize, matrixSize];
        for (int i = 0; i < matrixSize; i++)
            for (int j = 0; j < matrixSize; j++)
                source[i, j] = ((char)('a' + (i + j) % 33)).ToString();

        var helper = new WordSearchHelper("жирнеть", source);

        var stopwatch = Stopwatch.StartNew();
        var result = helper.Search();
        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds < 1000,
            $"Large matrix search took {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void WordSearchHelper_Search_SmallMatrix_ShouldBeInstant()
    {
        var source = new string[,]
        {
            { "ж", "и", "р" }, { "н", "е", "т" }, { "ь", " ", " " }
        };

        var helper = new WordSearchHelper("жирнеть", source);

        var stopwatch = Stopwatch.StartNew();
        var result = helper.Search();
        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds < 100,
            $"Small matrix search took {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void WordSearchHelper_Search_NonExistentWord_ShouldBeFast()
    {
        var source = new string[,]
        {
            { "а", "б", "в" }, { "г", "д", "е" }, { "ж", "з", "и" }
        };

        var helper = new WordSearchHelper("несуществующее", source);

        var stopwatch = Stopwatch.StartNew();
        var result = helper.Search();
        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds < 100,
            $"Non-existent word search took {stopwatch.ElapsedMilliseconds}ms");
    }

    #endregion
}