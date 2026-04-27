using Domain.Entity;
using Domain.Groups;

namespace Application.Repositories;

public class GroupRepository(IDbContext dbContext) : CrudRepository<Group, GroupUpdateDto>(dbContext),  IGroupRepository
{
    public Group[] Read(string[] names)
        => Storage.Where(x => names.Contains(x.Name)).ToArray();
}