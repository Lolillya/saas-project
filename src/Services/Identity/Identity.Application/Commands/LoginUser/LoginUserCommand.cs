using Identity.Application.DTOs;
using MediatR;

namespace Identity.Application.Commands.LoginUser
{
    public record LoginUserCommand(string Email, string Password) : IRequest<AuthResponse>;
}