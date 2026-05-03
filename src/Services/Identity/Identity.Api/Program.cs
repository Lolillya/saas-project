using Identity.Application.Commands.LoginUser;
using Identity.Application.Commands.RegisterUser;
using Identity.Infrastructure;
using MediatR;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/auth/register", async (RegisterUserCommand command, IMediator mediator) =>
{
    var result = await mediator.Send(command);
    return Results.Ok(result);
});

app.MapPost("/auth/login", async (LoginUserCommand command, IMediator mediator) =>
{
    var result = await mediator.Send(command);
    return Results.Ok(result);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}

app.UseHttpsRedirection();
app.Run();
