using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Identity.Application.Services;
using MediatR;

namespace Identity.Application.Commands.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResponse>
    {
        private readonly IIdentityRepository _repository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenServices _tokenServices;

        public LoginUserCommandHandler(IIdentityRepository repository, IPasswordHasher passwordHasher, ITokenServices tokenServices)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
            _tokenServices = tokenServices;
        }

        public async Task<AuthResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetUserByEmailAsync(
                request.Email.ToLowerInvariant().Trim(), cancellationToken
            );

            if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
                throw new InvalidOperationException("Invalid email or password.");

            if (!user.IsActive)
                throw new InvalidOperationException("User account is inactive.");

            var refreshToken = _tokenServices.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            await _repository.UpdateUserAsync(user, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            var tokenResult = _tokenServices.GenerateToken(user, user.Tenant.Slug, "Admim");

            return new AuthResponse(tokenResult.Token, refreshToken, tokenResult.ExpiresAt);
        }
    }
}