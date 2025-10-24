using MiniProjectManager.Application.DTOs.Auth;

namespace MiniProjectManager.Application.Interfaces;
public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}
