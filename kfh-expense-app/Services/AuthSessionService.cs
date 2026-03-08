using kfh_expense_app.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace kfh_expense_app.Services;

public class AuthSessionService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly JwtService _jwtService;
    public AuthSessionService(UserManager<AppUser> userManager,
        JwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    public string? Token { get; private set; }
    public string? Role { get; private set; }
    public string? UserId { get; private set; }
    public string? Username { get; private set; }

    public bool IsAuthenticated => !string.IsNullOrEmpty(UserId);

    public bool IsAdmin => Role == "Admin";
    public bool IsApprover => Role == "Approver" || Role == "Admin";

    public event Action? OnChange;

    public async Task<(bool ok, string error)> LoginAsync(string basicHeader)
    {

        if (!basicHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            return (false, "Invalid authorization header format.");
        }

        string credentials;
        try
        {
            credentials = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(basicHeader.Substring("Basic ".Length).Trim()));
        }
        catch
        {
            return (false, "Invalid base64 encoding in authorization header.");

        }
        var sep = credentials.IndexOf(':');
        if (sep < 0)
        {
            return (false, "Invalid credentials format.");
        }

        var username = credentials[..sep];
        var password = credentials[(sep + 1)..];

        var user = await _userManager.FindByNameAsync(username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
        {
            return (false, "Invalid username or password.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Employee";

        Token = _jwtService.GenerateToken(user, role);
        Role = role;
        UserId = user.Id;
        Username = user.UserName;

        return (true, string.Empty);
    }

    public void Logout()
    {
        Token = null;
        Role = null;
        UserId = null;
        Username = null;

        OnChange.Invoke();
    }
}
