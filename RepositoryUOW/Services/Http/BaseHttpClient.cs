using System.Text;
using Newtonsoft.Json;

namespace RepositoryUOW.Services.Http;

public class BaseHttpClient
{
    private readonly HttpClient _httpClient;

    protected BaseHttpClient(HttpClient httpClient) => _httpClient = httpClient;

    public BaseHttpClient() => _httpClient = new HttpClient();

    public async Task<T?> Get<T>(string uri)
    {
        var request = CreateRequest(HttpMethod.Get, uri);

        return await ExecuteRequest<T>(request).ConfigureAwait(false);
    }

    public async Task<T?> Post<T>(string uri, object content)
    {
        var request = CreateRequest(HttpMethod.Post, uri, content);

        return await ExecuteRequest<T>(request);
    }


    private static HttpRequestMessage CreateRequest(HttpMethod httpMethod, string uri, object? content = null)
    {
        var request = new HttpRequestMessage(httpMethod, uri);
        if (content == null) return request;

        var json = JsonConvert.SerializeObject(content);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        return request;
    }

    private async Task<T?> ExecuteRequest<T>(HttpRequestMessage request)
    {
        try
        {
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return string.IsNullOrEmpty(responseContent) ? default : JsonConvert.DeserializeObject<T>(responseContent);
        }
        catch (Exception ex) when (ex is ArgumentNullException || ex is InvalidOperationException || ex is HttpRequestException || ex is JsonException)
        {
            throw;
        }
    } 

    public async Task PostRequest(string address, string requestAsJson)
    {
        try
        {
            HttpContent contentPost = new StringContent(requestAsJson, Encoding.UTF8, "application/json");
           await _httpClient.PostAsync(new Uri(address), contentPost).ContinueWith((postTask) => { postTask.Result.EnsureSuccessStatusCode(); });
        }
        catch (Exception ex) when (ex is ArgumentNullException || ex is InvalidOperationException || ex is HttpRequestException || ex is JsonException)
        {
            throw;
        }
    }
}