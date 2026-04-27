using Domain.Entity;

namespace Domain.Groups;

public interface IGroupRepository : IRepositoryForModifiable<Group, GroupUpdateDto>
{
    public Group[] Read(string[] names);
}