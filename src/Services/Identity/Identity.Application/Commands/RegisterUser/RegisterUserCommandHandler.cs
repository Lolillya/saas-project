using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Identity.Application.Services;
using Identity.Domain.Entities;
using MediatR;

namespace Identity.Application.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponse>
    {
        private readonly IIdentityRepository _repository;
        private readonly ITokenServices _tokenServices;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterUserCommandHandler(IIdentityRepository repository, ITokenServices tokenServices, IPasswordHasher passwordHasher)
        {
            _repository = repository;
            _tokenServices = tokenServices;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (await _repository.EmailExistsAsync(request.Email, cancellationToken))
                throw new InvalidOperationException("Email already exists.");

            if (await _repository.SlugExistsAsync(request.TenantSlug, cancellationToken))
                throw new InvalidOperationException("Tenant slug already exists.");

            var tenant = new Tenants
            {
                FirstName = request.TenantFirstName,
                LastName = request.TenantLastName,
                Slug = request.TenantSlug.ToLowerInvariant().Trim(),
                IsActive = true,
                PlanId = request.PlanId,
                UpdatedAt = DateTime.UtcNow
            };

            tenant = await _repository.AddTenantAsync(tenant, cancellationToken);

            var refreshTokan = _tokenServices.GenerateRefreshToken();

            var user = new AspNetUsers
            {
                Email = request.Email.ToLowerInvariant().Trim(),
                PasswordHash = _passwordHasher.Hash(request.Password),
                CompanyName = request.CompanyName,
                IsActive = true,
                TenantId = tenant.TenantId,
                RefreshToken = refreshTokan,
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(7)
            };

            await _repository.AddUserAsync(user, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            var tokenResult = _tokenServices.GenerateToken(user, tenant.Slug, request.Role);

            return new AuthResponse(tokenResult.Token, refreshTokan, tokenResult.ExpiresAt);
        }
    }
}