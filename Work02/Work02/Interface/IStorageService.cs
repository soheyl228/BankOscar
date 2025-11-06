namespace Work02.Interface
{
    // Abstraction for persistent storage (localStorage wrapper)
    // Makes it possible to replace the storage implementation for tests.
    public interface IStorageService
    {
        Task SetItemAsync<T>(string key, T value);
        Task<T> GetItemAsync<T>(string key);
    }
}
