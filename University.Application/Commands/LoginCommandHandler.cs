namespace University.Application.Commands;

using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handler for the LoginCommand. Validates user credentials for Blazor login flow.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<LoginCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the LoginCommandHandler.
    /// </summary>
    /// <param name="userManager">The Identity user manager.</param>
    /// <param name="signInManager">The Identity sign-in manager.</param>
    /// <param name="logger">The logger instance.</param>
    public LoginCommandHandler(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        ILogger<LoginCommandHandler> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    /// <summary>
    /// Handles the login command by validating credentials.
    /// </summary>
    /// <param name="request">The login command containing email, password, and role.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>LoginResult indicating success/failure and user identity on success.</returns>
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            _logger.LogWarning("Login attempt failed: user not found ({Email})", request.Email);
            return new LoginResult { Success = false, Message = "Invalid email or password" };
        }

        // In interactive Blazor circuits we only validate credentials here.
        // Cookie sign-in must be performed from an HTTP request pipeline, not a circuit event.
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Login attempt failed: invalid password ({Email})", request.Email);
            return new LoginResult { Success = false, Message = "Invalid email or password" };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Unknown";

        if (!string.IsNullOrWhiteSpace(request.Role) && !string.Equals(role, request.Role, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Login attempt failed: role mismatch ({Email}), requested={RequestedRole}, actual={ActualRole}", request.Email, request.Role, role);
            return new LoginResult { Success = false, Message = "Role does not match this account" };
        }

        _logger.LogInformation("User credentials validated: {Email}, Role: {Role}", user.Email, role);
        return new LoginResult { Success = true, UserId = user.Id, Role = role };
    }
}
