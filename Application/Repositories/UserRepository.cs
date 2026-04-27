using Domain.Entity;
using Domain.Users;

namespace Application.Repositories;

public class UserRepository(IDbContext dbContext) : CrudRepository<User, UserUpdateDto>(dbContext), IUserRepository
{
    public void Create(User[] users)
    {
        Storage.AddRange(users);
        SaveChanges();
    }

    public List<User> Read(string[] logins)
    {
        var existUsersByPhone = Storage.Where(u => logins.AsEnumerable().Contains(u.Phone));
        var existUsersByEmail = Storage.Where(u => logins.AsEnumerable().Contains(u.Email));
        var existUsers = existUsersByPhone.Union(existUsersByEmail);
        return existUsers.ToList();
    }

    public List<User> Read(Guid[] ids)
        => Storage.Where(u => ids.AsEnumerable().Contains(u.Id)).ToList();

    public List<User> ReadTeachers()
        => Storage.Where(x => x.Roles.Contains(Role.Teacher)).ToList();

    public void UpdatePassword(Guid id, PasswordUpdateDto dto)
    {
        var user = Read(id) ?? throw new Exception("User not found");
        user.UpdatePassword(dto);
        SaveChanges();
    }
}