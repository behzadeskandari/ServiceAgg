namespace ServiceAggregator.Api.Services;
public class LoginService
{
    //private readonly List<(string Username, string Password)> _users =
    //[
    //    ("admin", "1234"),
    //];

    //public bool Validate(string username, string password)
    //{
    //    return _users.Any(u => u.Username == username && u.Password == password);
    //}
    private readonly List<(string Username, string Password, string Role)> _users =
    [
        ("admin", "1234", "Admin"),
    ];

    public string? Validate(string username, string password)
    {
        var user = _users.FirstOrDefault(u => u.Username == username && u.Password == password);
        return user == default ? null : user.Role;   // null یعنی ورود نامعتبر
    }
}
