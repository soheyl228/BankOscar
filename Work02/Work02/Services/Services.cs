using Microsoft.JSInterop;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Work02.Services
{
    // StorageService: small abstraction over browser localStorage using IJSRuntime.
    // Serializes objects using System.Text.Json and stores JSON strings under given keys.
    public class StorageService : IStorageService
    {
        private readonly IJSRuntime _jsRuntime;

        // Use camelCase and support enums as strings for stable serialization
        JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };

        public StorageService(IJSRuntime jsRuntime) => _jsRuntime = jsRuntime;

        // Persist an object to localStorage
        public async Task SetItemAsync<T>(string key, T value)
        {
            var json = JsonSerializer.Serialize(value, _jsonSerializerOptions);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
        }

        // Retrieve an object of type T from localStorage; returns default(T) if missing
        public async Task<T> GetItemAsync<T>(string key)
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
            if (string.IsNullOrWhiteSpace(json))
                return default;
            return JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions)!;
        }
    }
}
