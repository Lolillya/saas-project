using Identity.Application.Commands.LoginUser;
using Identity.Application.Commands.RegisterUser;
using Identity.Infrastructure;
using MediatR;

if (File.Exists(".env")) DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/auth/register", async (RegisterUserCommand command, IMediator mediator) =>
{
    try
    {
        var result = await mediator.Send(command);
        return Results.Ok(result);
    }
    catch (InvalidOperationException ex)
    {
        return Results.Problem(ex.Message, statusCode: 400);
    }
});

app.MapPost("/auth/login", async (LoginUserCommand command, IMediator mediator) =>
{
    try
    {
        var result = await mediator.Send(command);
        return Results.Ok(result);
    }
    catch (InvalidOperationException ex)
    {
        return Results.Problem(ex.Message, statusCode: 400);
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}

app.UseHttpsRedirection();
app.Run();
