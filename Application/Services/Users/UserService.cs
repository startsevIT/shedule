using Domain.Entity;
using Domain.Groups;
using Domain.Users;

namespace Application.Services.Users;

public class UserService(
    EntityStorage entityStorage,
    PasswordGeneratorService passwordGeneratorService,
    EmailService emailService,
    EmailBodyGeneratedService emailBodyGeneratedService,
    TokenService tokenService)
{
    public Task RegisterTeachers(TeacherRegisterDto[] dtos) => RegisterMany(dtos);

    public Task RegisterStudents(StudentRegisterDto[] dtos) => RegisterMany(dtos);

    public Task RegisterManager(ManagerRegisterDto[] dtos) => RegisterSingle(dtos);

    public Task RegisterEntrant(EntrantRegisterDto[] dtos) => RegisterSingle(dtos);

    public string Login(UserLoginDto dto)
        => Authenticate(dto.Login, dto.Password, out var user)
            ? tokenService.CreateToken(user!.Id, user.Roles.ToArray())
            : throw new Exception("Ошибка при входе");

    public UserReadDto GetAccount(Guid id)
    {
        var user = entityStorage.Read<User>(id)
                   ?? throw new Exception("Пользователь не найден");
        var dto = user.ToDto();
        if (user.Roles.Contains(Role.Student))
        {
            var group = entityStorage.Read<Group>(user.GroupId!.Value)
                        ?? throw new Exception("У студента указана несуществующая группа");
            dto.GroupName = group.Name;
        }

        return dto;
    }

    public void Update(Guid id, UserUpdateDto dto)
    {
        var user = entityStorage.Read<User>(id)
                   ?? throw new Exception("Пользователь не найден");
        user.Update(dto);
        entityStorage.Update(user);
    }

    public void UpdatePassword(Guid id, PasswordUpdateDto dto)
    {
        var user = entityStorage.Read<User>(id)
                   ?? throw new Exception("Пользователь не найден");
        user.UpdatePassword(dto);
        entityStorage.Update(user);
    }

    private bool Authenticate(string login, string password, out User? user)
        => Identify([login], out user) && user!.VerifyPassword(password);

    private Task RegisterSingle<TRegisterDto>(TRegisterDto[] dtos)
        where TRegisterDto : RegisterDto, IHavePassword
    {
        var dto = dtos.Single();
        if (Identify([dto.Email, dto.Phone], out _))
            throw new Exception("Пользователь уже существует");
        entityStorage.Create(User.Create(dto, dto.Password));
        return Task.CompletedTask;
    }

    private async Task RegisterMany<TRegisterDto>(TRegisterDto[] dtos)
        where TRegisterDto : RegisterDto
    {
        var existUsers = entityStorage.Select<User>()
            .Where(x => dtos.Select(p => p.Phone).Contains(x.Phone)
                        || dtos.Select(e => e.Email).Contains(x.Email))
            .ToArray();
        if (existUsers.Length != 0)
            throw new Exception("При регистрации студентов обнаружили совпадения в номерах телефонов или почтах");

        List<User> usersToCreate = [];
        List<User> failUsers = [];
        foreach (var dto in dtos)
        {
            var password = passwordGeneratorService.Generate();
            var user = User.Create(dto, password);

            if (await emailService.SendAsync(emailBodyGeneratedService.GenerateEmailBody(user.FullName, user.Email, password), user.Email))
                usersToCreate.Add(user);
            else
                failUsers.Add(user);
        }

        if (failUsers.Count != 0)
            throw new Exception("При отправке сообщений с паролями возникла ошибка");

        if (usersToCreate.Count != 0)
            entityStorage.Create([.. usersToCreate]);
    }

    private bool Identify(string[] identificators, out User? user)
    {
        var users = entityStorage.Select<User>()
            .Where(x => identificators.Contains(x.Phone) || identificators.Contains(x.Email))
            .ToArray();
        user = users.Length switch
        {
            1 => users[0],
            > 1 => throw new Exception($"Пользователей по этим данным {users.Length} шт. Надо разбираться"),
            _ => null
        };
        return users.Length == 1;
    }
}

public record UserLoginDto(
    string Login,
    string Password);

//Аутентификация - понять, не врет ли чувак, что он пользователь
//Идентификация - понять, есть ли по идентификатору такой чувак
//Авторизация - будет на уровне middleware
//Верификация - понять, не врет ли чувак по данным, которые он указывал(пароль), может быть номер или мыло
//Вход - получение ключа доступа к данным (Есть отлельный сервис)
//Регистрация - добавляем пользователя в систему (Разделил на разные сервисы)