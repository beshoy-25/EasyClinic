using EasyClinic.API.Auth;
using EasyClinic.API.DTOs.Auth;
using M04.MinimalFluentValidation.Filters;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/login", Login)
             .AddEndpointFilter<ValidationFilter<LoginDTO>>()
             .AllowAnonymous();
    }

    private static IResult Login(LoginDTO dto, IJwtTokenService tokenService)
    {
        var user = FakeUserStore.Validate(dto.Email, dto.Password);

        if (user is null)
            return Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Invalid credentials",
                detail: "Email or password is incorrect.");

        return Results.Ok(new LoginResponseDTO(tokenService.GenerateToken(user)));
    }
}