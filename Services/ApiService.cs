using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Rockstar.Admin.WPF.Services.Interfaces;
using System.Diagnostics;

namespace Rockstar.Admin.WPF.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        public string BaseUrl { get; }

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            BaseUrl = _httpClient.BaseAddress?.ToString() ?? "http://localhost:5143/api/";

            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public void SetAuthToken(string? token)
        {
            if (string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
                Debug.WriteLine("🔑 Auth token cleared");
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
                Debug.WriteLine($"🔑 Auth token set: {token.Substring(0, Math.Min(20, token.Length))}...");
            }
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                Debug.WriteLine($"📥 GET {BaseUrl}{endpoint}");
                Debug.WriteLine($"🔑 Token: {_httpClient.DefaultRequestHeaders.Authorization}");

                var response = await _httpClient.GetAsync(endpoint);

                Debug.WriteLine($"📊 Response Status: {(int)response.StatusCode} {response.StatusCode}");

                var json = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"📄 Response Body: {json}");

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"❌ GET failed: {response.StatusCode}");
                    return default;
                }

                return JsonSerializer.Deserialize<T>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"💥 GET error: {ex.Message}");
                return default;
            }
        }

        public async Task<T?> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                Debug.WriteLine($"📤 POST {BaseUrl}{endpoint}");
                Debug.WriteLine($"🔑 Token: {_httpClient.DefaultRequestHeaders.Authorization}");

                var json = JsonSerializer.Serialize(data, _jsonOptions);
                Debug.WriteLine($"📄 Request Body: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);

                Debug.WriteLine($"📊 Response Status: {(int)response.StatusCode} {response.StatusCode}");

                var responseJson = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"📄 Response Body: {responseJson}");

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"❌ POST failed: {response.StatusCode}");
                    return default;
                }

                return JsonSerializer.Deserialize<T>(responseJson, _jsonOptions);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"💥 POST error: {ex.Message}");
                return default;
            }
        }

        public async Task<T?> PutAsync<T>(string endpoint, object data)
        {
            try
            {
                Debug.WriteLine($"📤 PUT {BaseUrl}{endpoint}");

                var json = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(endpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"❌ PUT failed: {response.StatusCode}");
                    return default;
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(responseJson, _jsonOptions);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"💥 PUT error: {ex.Message}");
                return default;
            }
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                Debug.WriteLine($"📤 DELETE {BaseUrl}{endpoint}");

                var response = await _httpClient.DeleteAsync(endpoint);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"💥 DELETE error: {ex.Message}");
                return false;
            }
        }
    }
}