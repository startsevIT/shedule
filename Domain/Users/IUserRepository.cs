using Domain.Entity;

namespace Domain.Users;

public interface IUserRepository : ICrudModifiable<User, UserUpdateDto>
{
    void Create(User[] users);
    List<User> Read(string[] logins);
    List<User> Read(Guid[] ids);
    List<User> ReadTeachers();
    void UpdatePassword(Guid id, PasswordUpdateDto dto);
}