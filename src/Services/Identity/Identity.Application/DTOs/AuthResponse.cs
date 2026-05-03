namespace Identity.Application.DTOs
{
    public class AuthResponse
    (
        string AccessToken,
        string RefreshToken,
        DateTime ExpiresAt
    );
}