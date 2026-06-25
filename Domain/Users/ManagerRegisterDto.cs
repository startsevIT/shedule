using System.ComponentModel.DataAnnotations;

namespace Domain.Users;

public record ManagerRegisterDto(
    [Length(2, 50, ErrorMessage = "Длина фамилии должна быть от {1} до {2}")]
    string FirstName,
    [Length(2, 50, ErrorMessage = "Длина имени должна быть от {1} до {2}")]
    string Name,
    [Length(2, 50, ErrorMessage = "Длина имени должна быть от {1} до {2}")]
    string Patronymic,
    [EmailAddress(ErrorMessage = "Некорректная почта")]
    string Email,
    [Phone(ErrorMessage = "Некорректный номер телефона")]
    string Phone,
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{10,}$")]
    string Password) : RegisterDto(FirstName, Name, Patronymic, Phone, Email), IHavePassword;