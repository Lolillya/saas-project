using Identity.Application.DTOs;
using MediatR;

namespace Identity.Application.Commands.RegisterUser
{
    public record RegisterUserCommand
    (
        string Email,
        string Password,
        string CompanyName,
        string TenantFirstName,
        string TenantLastName,
        string TenantSlug,
        int PlanId,
        string Role = "Admin"
    ) : IRequest<AuthResponse>;
}