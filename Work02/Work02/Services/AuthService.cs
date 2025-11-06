using System.Threading.Tasks;
using Work02.Interface;

namespace Work02.Services
{
    public class AuthService : IAuthService
    {
        private const string StorageKey = "banken.authenticated";
        private readonly IStorageService _storageService;
        public bool IsAuthenticated { get; private set; }

        public AuthService(IStorageService storageService) => _storageService = storageService;

        // Read persisted auth state from storage so user stays signed in between reloads
        public async Task InitializeAsync()
        {
            var val = await _storageService.GetItemAsync<bool?>(StorageKey);
            IsAuthenticated = val ?? false;
        }

        // PIN is hardcoded as "1234" for this demo
        public async Task<bool> ValidatePinAsync(string pin)
        {
            if (pin == "1234")
            {
                IsAuthenticated = true;
                await _storageService.SetItemAsync(StorageKey, true);
                return true;
            }

            IsAuthenticated = false;
            await _storageService.SetItemAsync(StorageKey, false);
            return false;
        }

        public async Task SignOutAsync()
        {
            IsAuthenticated = false;
            await _storageService.SetItemAsync(StorageKey, false);
        }
    }
}
