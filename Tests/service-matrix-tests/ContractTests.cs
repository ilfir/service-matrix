using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using service_matrix_tests;
using Xunit;

namespace service_matrix_contract_tests;

/// <summary>
/// API Contract Tests - Verify API response schemas and structure consistency.
/// These tests ensure backward compatibility by checking that response shapes remain stable.
/// </summary>
public class ContractTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ContractTests(TestWebApplicationFactory factory)
     {
         _client = factory.CreateClient();
     }

     #region Search Endpoint Contract Tests

     [Fact]
     public async Task Search_Response_ShouldBeJsonObject()
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

         var response = await _client.PostAsJsonAsync("/words/Search", requestBody);
         var responseString = await response.Content.ReadAsStringAsync();
         var jsonDocument = JsonDocument.Parse(responseString);

         Assert.Equal(JsonValueKind.Object, jsonDocument.RootElement.ValueKind);
     }

     [Fact]
     public async Task Search_Response_Values_ShouldBeObjects()
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

         var response = await _client.PostAsJsonAsync("/words/Search", requestBody);
         var responseString = await response.Content.ReadAsStringAsync();
         var jsonDocument = JsonDocument.Parse(responseString);

         foreach (var property in jsonDocument.RootElement.EnumerateObject())
          {
             Assert.Equal(JsonValueKind.Object, property.Value.ValueKind);
          }
     }

     [Fact]
     public async Task Search_Response_ShouldBeConsistentAcrossMultipleCalls()
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

         var response1 = await _client.PostAsJsonAsync("/words/Search", requestBody);
         var response2 = await _client.PostAsJsonAsync("/words/Search", requestBody);
         var response3 = await _client.PostAsJsonAsync("/words/Search", requestBody);

         var str1 = await response1.Content.ReadAsStringAsync();
         var str2 = await response2.Content.ReadAsStringAsync();
         var str3 = await response3.Content.ReadAsStringAsync();

         var doc1 = JsonDocument.Parse(str1);
         var doc2 = JsonDocument.Parse(str2);
         var doc3 = JsonDocument.Parse(str3);

         Assert.Equal(doc1.RootElement.ValueKind, doc2.RootElement.ValueKind);
         Assert.Equal(doc2.RootElement.ValueKind, doc3.RootElement.ValueKind);
     }

     #endregion

     #region Update Endpoint Contract Tests

     [Fact]
     public async Task Update_Response_ShouldBeInteger()
     {
         var requestBody = new { Words = new List<string> { "testword" }, Include = true };

         var response = await _client.PostAsJsonAsync("/words/Update", requestBody);
         var responseString = await response.Content.ReadAsStringAsync();

         Assert.True(int.TryParse(responseString, out _), "Update response should be parseable as integer");
     }

     [Fact]
     public async Task Update_Response_WhenWordsEmpty_ShouldBeZero()
     {
         var requestBody = new { Words = Array.Empty<string>(), Include = true };

         var response = await _client.PostAsJsonAsync("/words/Update", requestBody);
         var responseString = await response.Content.ReadAsStringAsync();

         Assert.True(int.TryParse(responseString, out int count), "Update response should be parseable as integer");
         Assert.Equal(0, count);
     }

     [Fact]
     public async Task Update_Response_WhenIncludeTrue_ShouldReturnNonNegativeCount()
     {
         var requestBody = new { Words = new List<string> { "abc" }, Include = true };

         var response = await _client.PostAsJsonAsync("/words/Update", requestBody);
         var responseString = await response.Content.ReadAsStringAsync();

         Assert.True(int.TryParse(responseString, out int count), "Update response should be parseable as integer");
         Assert.True(count >= 0, "Count should be non-negative");
     }

     #endregion

     #region List Endpoint Contract Tests

     [Fact]
     public async Task List_Response_ShouldBeJsonArray()
     {
         var response = await _client.GetAsync("/words/List?include=true");
         var responseString = await response.Content.ReadAsStringAsync();
         var jsonDocument = JsonDocument.Parse(responseString);

         Assert.Equal(JsonValueKind.Array, jsonDocument.RootElement.ValueKind);
     }

     [Fact]
     public async Task List_Response_ArrayElements_ShouldBeStrings()
     {
         var response = await _client.GetAsync("/words/List?include=true");
         var responseString = await response.Content.ReadAsStringAsync();
         var jsonDocument = JsonDocument.Parse(responseString);

         foreach (var element in jsonDocument.RootElement.EnumerateArray())
          {
             Assert.Equal(JsonValueKind.String, element.ValueKind);
          }
     }

     [Fact]
     public async Task List_Response_ShouldBeConsistentAcrossMultipleCalls()
     {
         var response1 = await _client.GetAsync("/words/List?include=true");
         var response2 = await _client.GetAsync("/words/List?include=true");

         var str1 = await response1.Content.ReadAsStringAsync();
         var str2 = await response2.Content.ReadAsStringAsync();

         var doc1 = JsonDocument.Parse(str1);
         var doc2 = JsonDocument.Parse(str2);

         Assert.Equal(JsonValueKind.Array, doc1.RootElement.ValueKind);
         Assert.Equal(JsonValueKind.Array, doc2.RootElement.ValueKind);
     }

     [Fact]
     public async Task List_Response_IncludeFalse_ShouldReturnArray()
     {
         var response = await _client.GetAsync("/words/List?include=false");
         var responseString = await response.Content.ReadAsStringAsync();
         var jsonDocument = JsonDocument.Parse(responseString);

         Assert.Equal(JsonValueKind.Array, jsonDocument.RootElement.ValueKind);
     }

     #endregion

     #region Merge Endpoint Contract Tests

     [Fact]
     public async Task Merge_Response_ShouldBeJsonObject()
     {
         var response = await _client.PostAsync("/words/Merge", null);
         var responseString = await response.Content.ReadAsStringAsync();
         var jsonDocument = JsonDocument.Parse(responseString);

         Assert.Equal(JsonValueKind.Object, jsonDocument.RootElement.ValueKind);
     }

     [Fact]
     public async Task Merge_Response_ShouldHaveAddedCountProperty()
     {
         var response = await _client.PostAsync("/words/Merge", null);
         var responseString = await response.Content.ReadAsStringAsync();
         var jsonDocument = JsonDocument.Parse(responseString);

         Assert.True(jsonDocument.RootElement.TryGetProperty("addedCount", out _),
              "Response should contain 'addedCount' property");
     }

     [Fact]
     public async Task Merge_Response_ShouldHaveRemovedCountProperty()
     {
         var response = await _client.PostAsync("/words/Merge", null);
         var responseString = await response.Content.ReadAsStringAsync();
         var jsonDocument = JsonDocument.Parse(responseString);

         Assert.True(jsonDocument.RootElement.TryGetProperty("removedCount", out _),
              "Response should contain 'removedCount' property");
     }

     [Fact]
     public async Task Merge_Response_AddedCount_ShouldBeNumber()
     {
         var response = await _client.PostAsync("/words/Merge", null);
         var responseString = await response.Content.ReadAsStringAsync();
         var jsonDocument = JsonDocument.Parse(responseString);

         var addedCount = jsonDocument.RootElement.GetProperty("addedCount");
         Assert.Equal(JsonValueKind.Number, addedCount.ValueKind);
     }

     [Fact]
     public async Task Merge_Response_RemovedCount_ShouldBeNumber()
     {
         var response = await _client.PostAsync("/words/Merge", null);
         var responseString = await response.Content.ReadAsStringAsync();
         var jsonDocument = JsonDocument.Parse(responseString);

         var removedCount = jsonDocument.RootElement.GetProperty("removedCount");
         Assert.Equal(JsonValueKind.Number, removedCount.ValueKind);
     }

     [Fact]
     public async Task Merge_Response_ShouldBeConsistentAcrossMultipleCalls()
     {
         var response1 = await _client.PostAsync("/words/Merge", null);
         var response2 = await _client.PostAsync("/words/Merge", null);

         var str1 = await response1.Content.ReadAsStringAsync();
         var str2 = await response2.Content.ReadAsStringAsync();

         var doc1 = JsonDocument.Parse(str1);
         var doc2 = JsonDocument.Parse(str2);

         Assert.Equal(JsonValueKind.Object, doc1.RootElement.ValueKind);
         Assert.Equal(JsonValueKind.Object, doc2.RootElement.ValueKind);
     }

     #endregion

     #region CleanMerge Endpoint Contract Tests

     [Fact]
     public async Task CleanMerge_Response_ShouldBeString()
     {
         var response = await _client.GetAsync("/words/CleanMerge");
         var responseString = await response.Content.ReadAsStringAsync();

         Assert.Contains("BEFORE:", responseString);
         Assert.Contains("AFTER:", responseString);
     }

     [Fact]
     public async Task CleanMerge_Response_ShouldContainNumbers()
     {
         var response = await _client.GetAsync("/words/CleanMerge");
         var responseString = await response.Content.ReadAsStringAsync();

         var beforeMatch = System.Text.RegularExpressions.Regex.Match(responseString, @"BEFORE:\s*(\d+)");
         var afterMatch = System.Text.RegularExpressions.Regex.Match(responseString, @"AFTER:\s*(\d+)");

         Assert.True(beforeMatch.Success, "Response should contain BEFORE count");
         Assert.True(afterMatch.Success, "Response should contain AFTER count");
     }

     [Fact]
     public async Task CleanMerge_Response_ShouldBeConsistentAcrossMultipleCalls()
     {
         var response1 = await _client.GetAsync("/words/CleanMerge");
         var response2 = await _client.GetAsync("/words/CleanMerge");

         var str1 = await response1.Content.ReadAsStringAsync();
         var str2 = await response2.Content.ReadAsStringAsync();

         Assert.Contains("BEFORE:", str1);
         Assert.Contains("AFTER:", str1);
         Assert.Contains("BEFORE:", str2);
         Assert.Contains("AFTER:", str2);
     }

     #endregion

     #region LookupWord Endpoint Contract Tests

     [Fact]
     public async Task LookupWord_Response_ShouldBeJsonArray()
     {
         var response = await _client.GetAsync("/words/LookupWord?word=test&exactMatch=true");
         var responseString = await response.Content.ReadAsStringAsync();
         var jsonDocument = JsonDocument.Parse(responseString);

         Assert.Equal(JsonValueKind.Array, jsonDocument.RootElement.ValueKind);
     }

     [Fact]
     public async Task LookupWord_Response_ArrayElements_ShouldBeObjects()
     {
         var response = await _client.GetAsync("/words/LookupWord?word=test&exactMatch=true");
         var responseString = await response.Content.ReadAsStringAsync();
         var jsonDocument = JsonDocument.Parse(responseString);

         foreach (var element in jsonDocument.RootElement.EnumerateArray())
          {
             Assert.Equal(JsonValueKind.Object, element.ValueKind);
          }
     }

     [Fact]
     public async Task LookupWord_Response_ShouldHaveWordAndLocationProperties()
     {
         var response = await _client.GetAsync("/words/LookupWord?word=test&exactMatch=true");
         var responseString = await response.Content.ReadAsStringAsync();
         var jsonDocument = JsonDocument.Parse(responseString);

         foreach (var element in jsonDocument.RootElement.EnumerateArray())
          {
             Assert.True(element.TryGetProperty("word", out _), "Each lookup result should have 'word' property");
             Assert.True(element.TryGetProperty("location", out _), "Each lookup result should have 'location' property");
          }
     }

     [Fact]
     public async Task LookupWord_Response_ShouldBeConsistentAcrossMultipleCalls()
     {
         var response1 = await _client.GetAsync("/words/LookupWord?word=test&exactMatch=true");
         var response2 = await _client.GetAsync("/words/LookupWord?word=test&exactMatch=true");

         var str1 = await response1.Content.ReadAsStringAsync();
         var str2 = await response2.Content.ReadAsStringAsync();

         var doc1 = JsonDocument.Parse(str1);
         var doc2 = JsonDocument.Parse(str2);

         Assert.Equal(JsonValueKind.Array, doc1.RootElement.ValueKind);
         Assert.Equal(JsonValueKind.Array, doc2.RootElement.ValueKind);
     }

     #endregion

     #region HTTP Status Code Contract Tests

     [Fact]
     public async Task Search_Endpoint_ShouldReturn200OK()
     {
         var requestBody = new
          {
             LettersMatrix = new List<List<string>> { new List<string> { "a", "b" } }
          };

         var response = await _client.PostAsJsonAsync("/words/Search", requestBody);
         Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
     }

     [Fact]
     public async Task Update_Endpoint_ShouldReturn200OK()
     {
         var requestBody = new { Words = new List<string> { "test" }, Include = true };
         var response = await _client.PostAsJsonAsync("/words/Update", requestBody);
         Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
     }

     [Fact]
     public async Task List_Endpoint_ShouldReturn200OK()
     {
         var response = await _client.GetAsync("/words/List?include=true");
         Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
     }

     [Fact]
     public async Task Merge_Endpoint_ShouldReturn200OK()
     {
         var response = await _client.PostAsync("/words/Merge", null);
         Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
     }

     [Fact]
     public async Task CleanMerge_Endpoint_ShouldReturn200OK()
     {
         var response = await _client.GetAsync("/words/CleanMerge");
         Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
     }

     [Fact]
     public async Task LookupWord_Endpoint_ShouldReturn200OK()
     {
         var response = await _client.GetAsync("/words/LookupWord?word=test&exactMatch=true");
         Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
     }

     #endregion
}