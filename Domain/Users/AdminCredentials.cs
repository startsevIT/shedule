namespace Domain.Users;

public class AdminCredentials
{
    public string FirstName { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Patronymic { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Role { get; set; } = null!;
}