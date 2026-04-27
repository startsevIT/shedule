using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using Domain.Entity;

namespace Domain.Users;

[Table("users")]
public class User : ModifyEntity<UserUpdateDto>
{
    [Column("first_name")]
    private string FirstName { get; set; } = null!;

    [Column("name")]
    private string Name { get; set; } = null!;

    [Column("patronymic")]
    private string Patronymic { get; set; } = null!;

    [Column("email")]
    public string Email { get; private init; } = null!;

    [Column("phone")]
    public string Phone { get; private init; } = null!;

    [Column("hashed_password")]
    private string HashedPassword { get; set; } = null!;

    [Column("salt")]
    private string Salt { get; set; } = null!;

    [Column("roles")]
    public List<string> Roles { get; private init; } = null!;

    [Column("group_id")]
    public Guid? GroupId { get; private set; }

    public string Initials => $"{FirstName} {Name[0]}.{Patronymic[0]}.";
    public string FullName => $"{FirstName} {Name} {Patronymic}";

    private static (string hashedPassword, string salt) HashPassword(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var saltBytes = new byte[32];
        rng.GetBytes(saltBytes);

        var computedHash = ComputedHash(password, saltBytes);

        return (Convert.ToBase64String(computedHash), Convert.ToBase64String(saltBytes));
    }

    private static byte[] ComputedHash(string password, byte[] saltBytes)
        => Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, 10000, HashAlgorithmName.SHA256, 32);

    public static User Create(RegisterDto dto, string password)
    {
        var (hashedPassword, salt) = HashPassword(password);
        return new User
        {
            FirstName = dto.FirstName,
            Name = dto.Name,
            Patronymic = dto.Patronymic,
            Email = dto.Email,
            Phone = dto.Phone,
            HashedPassword = hashedPassword,
            Salt = salt,
            Roles = dto switch
            {
                ManagerRegisterDto => [Role.Manager],
                StudentRegisterDto => [Role.Student],
                TeacherRegisterDto => [Role.Teacher],
                EntrantRegisterDto => [Role.Entrant],
                _ => throw new ArgumentOutOfRangeException(nameof(dto), dto, null)
            },
            GroupId = dto is StudentRegisterDto s ? s.GroupId : null
        };
    }

    public UserReadDto ToDto()
        => new(FirstName: FirstName, Name, Patronymic, Email, Phone, Roles);

    public bool VerifyPassword(string password)
    {
        var hashBytes = Convert.FromBase64String(HashedPassword);
        var saltBytes = Convert.FromBase64String(Salt);

        var computedHash = ComputedHash(password, saltBytes);

        var diff = (uint)hashBytes.Length ^ (uint)computedHash.Length;

        for (var i = 0; i < hashBytes.Length && i < computedHash.Length; i++)
            diff |= (uint)(hashBytes[i] ^ computedHash[i]);

        return diff == 0;
    }

    public void UpdatePassword(PasswordUpdateDto dto)
    {
        if (!VerifyPassword(dto.OldPassword))
            throw new Exception("Пароли не совпадают");
        var (hashedPassword, salt) = HashPassword(dto.NewPassword);
        HashedPassword = hashedPassword;
        Salt = salt;
    }

    public override void Update(UserUpdateDto dto)
    {
        var dtoAddedRoles = dto.AddedRoles ?? [];
        var dtoRemovedRoles = dto.RemovedRoles ?? [];

        if (dtoAddedRoles.Intersect(dtoRemovedRoles).Any())
            throw new Exception("Попытка удалить и добавить одни и те же роли");

        foreach (var role in dtoAddedRoles)
            Roles.Add(role);

        foreach (var role in dtoRemovedRoles)
        {
            if (role == Role.Student)
                GroupId = null;
            Roles.Remove(role);
        }

        if (Roles.Contains(Role.Student))
            GroupId = dto.GroupId ?? GroupId;

        FirstName = dto.FirstName ?? FirstName;
        Name = dto.Name ?? Name;
        Patronymic = dto.Patronymic ?? Patronymic;
    }
}

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

public abstract record RegisterDto(
    string FirstName,
    string Name,
    string Patronymic,
    string Phone,
    string Email);

public interface IHavePassword
{
    public string Password { get; }
}

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

public record EntrantRegisterDto(
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

public record TeacherRegisterDto(
    [Length(2, 50, ErrorMessage = "Длина фамилии должна быть от {1} до {2}")]
    string FirstName,
    [Length(2, 50, ErrorMessage = "Длина имени должна быть от {1} до {2}")]
    string Name,
    [Length(2, 50, ErrorMessage = "Длина имени должна быть от {1} до {2}")]
    string Patronymic,
    [EmailAddress(ErrorMessage = "Некорректная почта")]
    string Email,
    [Phone(ErrorMessage = "Некорректный номер телефона")]
    string Phone) : RegisterDto(FirstName, Name, Patronymic, Phone, Email);

public record StudentRegisterDto(
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
    [Required(ErrorMessage = "Не указан номер группы")]
    Guid GroupId) : RegisterDto(FirstName, Name, Patronymic, Phone, Email);

public record PasswordUpdateDto(
    string OldPassword,
    string NewPassword);

public record UserUpdateDto(
    string? FirstName,
    string? Name,
    string? Patronymic,
    HashSet<string>? AddedRoles,
    HashSet<string>? RemovedRoles,
    Guid? GroupId = null) : UpdateDto;