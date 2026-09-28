namespace ServiceAggregator.Api.Services;
public class LoginService
{
    private readonly List<(string Username, string Password)> _users =
    [
        ("admin", "1234"),
    ];

    public bool Validate(string username, string password)
    {
        return _users.Any(u => u.Username == username && u.Password == password);
    }
}
