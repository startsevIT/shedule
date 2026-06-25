namespace Domain.Users;

public record PasswordUpdateDto(
    string OldPassword,
    string NewPassword);