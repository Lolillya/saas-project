using Identity.Application.Commands.LoginUser;
using Identity.Application.Commands.RegisterUser;
using Identity.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Serilog;
using Identity.Infrastructure.Data;
using Identity.Application.Interfaces;
using Identity.Application.Services;
using Identity.Application.DTOs;

if (File.Exists(".env")) DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(ctx.Configuration["Serilog:Seq:ServerUrl"] ?? "http://localhost:5341"));

builder.Services.AddProblemDetails();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    await db.Database.MigrateAsync();
}

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

app.MapPost("/auth/refresh", async (HttpContext ctx, IIdentityRepository repo, ITokenServices tokenSvc, CancellationToken ct) =>
{
    var body = await ctx.Request.ReadFromJsonAsync<RefreshRequest>(ct);
    if (body is null) return Results.BadRequest();

    var user = await repo.GetUserByRefreshTokenAsync(body.RefreshToken, ct);
    if (user is null || user.RefreshTokenExpiry < DateTime.UtcNow)
        return Results.Problem("Invalid or expired refresh token", statusCode: 401);

    var role = await repo.GetUserRoleAsync(user.Id, user.TenantId, ct) ?? "Admin";
    var newRefresh = tokenSvc.GenerateRefreshToken();
    user.RefreshToken = newRefresh;
    user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
    await repo.UpdateUserAsync(user, ct);
    await repo.SaveChangesAsync(ct);

    var tokenResult = tokenSvc.GenerateToken(user, user.Tenant.Slug, role);
    return Results.Ok(new AuthResponse(tokenResult.Token, newRefresh, tokenResult.ExpiresAt));
});

app.MapPost("/auth/logout", async (HttpContext ctx, IIdentityRepository repo, CancellationToken ct) =>
{
    var body = await ctx.Request.ReadFromJsonAsync<RefreshRequest>(ct);
    if (body is null) return Results.BadRequest();

    var user = await repo.GetUserByRefreshTokenAsync(body.RefreshToken, ct);
    if (user is null) return Results.Ok();

    user.RefreshToken = null;
    user.RefreshTokenExpiry = null;
    await repo.UpdateUserAsync(user, ct);
    await repo.SaveChangesAsync(ct);
    return Results.Ok();
}).RequireAuthorization();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.Run();
