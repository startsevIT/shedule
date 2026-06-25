namespace Domain.Users;

public record UserReadDto(
    string FirstName,
    string Name,
    string Patronymic,
    string Email,
    string Phone,
    List<string> Roles)
{
    public string? GroupName { get; set; }
};