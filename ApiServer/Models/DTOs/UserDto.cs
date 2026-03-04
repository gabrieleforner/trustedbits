namespace Trustedbits.ApiServer.Models.DTOs;

/// <summary>
/// This struct describe the fields that must be
/// provided in order to log in to the IdP
/// </summary>
public class LoginRequestDto
{
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? PhoneNumber { get; set; }
    
    public string Password { get; set; }
}

/// <summary>
/// This struct describe the fields that must be
/// provided in order to create a new account to
/// the IdP
/// </summary>
public class SignupRequestDto
{
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Username { get; set; }
    public string Password { get; set; }
}

public class UserDto
{
    public string? Email { get; set; } = null;
    public string? PhoneNumber { get; set; } = null;
    public string? Username { get; set; } = null;
}