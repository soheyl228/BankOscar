using System.Threading.Tasks;

namespace Work02.Interface
{
    public interface IAuthService
    {
        Task InitializeAsync();
        bool IsAuthenticated { get; }
        Task<bool> ValidatePinAsync(string pin);
        Task SignOutAsync();
    }
}   