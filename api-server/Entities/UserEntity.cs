namespace api_server.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
}

public class UserSignupData
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string? Username { get; set; }
    public string Password { get; set; }
}

public class UserLoginData
{
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string Password { get; set; }
}