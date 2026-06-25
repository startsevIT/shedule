using System.Security.Cryptography;
using Domain.Entity;
using Domain.Entity.Entities;

namespace Domain.Users;

public class User : ModifyEntity<UserUpdateDto>
{
    private string FirstName { get; set; } = null!;
    private string Name { get; set; } = null!;
    private string Patronymic { get; set; } = null!;
    public string Email { get; private init; } = null!;
    public string Phone { get; private init; } = null!;
    private string HashedPassword { get; set; } = null!;
    private string Salt { get; set; } = null!;
    public List<string> Roles { get; private init; } = null!;
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

    public static User CreateAdmin(AdminCredentials credentials)
    {
        var (hashedPassword, salt) = HashPassword(credentials.Password);
        return new User
        {
            FirstName = credentials.FirstName,
            Name = credentials.Name,
            Patronymic = credentials.Patronymic,
            Email = credentials.Email,
            Phone = credentials.Phone,
            HashedPassword = hashedPassword,
            Salt = salt,
            Roles = [credentials.Role]
        };
    }

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

        foreach (var role in dtoAddedRoles.Where(x => !Roles.Contains(x)))
            Roles.Add(role);

        foreach (var role in dtoRemovedRoles.Where(x => Roles.Contains(x)))
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