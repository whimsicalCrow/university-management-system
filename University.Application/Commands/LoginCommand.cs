namespace University.Application.Commands;

using MediatR;

/// <summary>
/// Command to authenticate a user and establish a session.
/// </summary>
public class LoginCommand : IRequest<LoginResult>
{
    /// <summary>
    /// Gets or sets the email address of the user attempting to log in.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Gets or sets the password of the user attempting to log in.
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// Gets or sets an optional role context for login.
    /// </summary>
    public string? Role { get; set; }
}

/// <summary>
/// Result of a login command execution.
/// </summary>
public class LoginResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the login was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets an error or status message.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the ID of the authenticated user (on success).
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Gets or sets the role of the authenticated user (on success).
    /// </summary>
    public string? Role { get; set; }
}
