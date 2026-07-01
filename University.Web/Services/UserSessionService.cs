namespace University.Web.Services;

public enum UserRole
{
    Student,
    Professor
}

/// <summary>
/// Scoped in-memory session: one instance per Blazor Server circuit (= per browser tab).
/// </summary>
public class UserSessionService
{
    public string? UserName { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsAuthenticated => UserName is not null;

    public event Action? OnChange;

    public void Login(string userName, UserRole role)
    {
        UserName = string.IsNullOrWhiteSpace(userName)
            ? (role == UserRole.Student ? "Student User" : "Professor User")
            : userName.Trim();
        Role = role;
        OnChange?.Invoke();
    }

    public void Logout()
    {
        UserName = null;
        OnChange?.Invoke();
    }
}
