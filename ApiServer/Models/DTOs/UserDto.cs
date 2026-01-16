namespace ApiServer.Models.DTOs;

public class UserLoginRequestDto
{
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? PhoneNumber { get; set; }
    
    public string Password { get; set; } = string.Empty;
}

public class UserSignupRequestDto
{
    public string Email { get; set; } = string.Empty;
    
    public string? PhoneNumber { get; set; }
    public string? Username { get; set; }
    
    public string Password { get; set; } = string.Empty;
}