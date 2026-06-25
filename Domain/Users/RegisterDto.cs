namespace Domain.Users;

public abstract record RegisterDto(
    string FirstName,
    string Name,
    string Patronymic,
    string Phone,
    string Email);