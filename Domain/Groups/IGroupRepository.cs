using Domain.Entity;

namespace Domain.Groups;

public interface IGroupRepository : ICrudRepository<Group, GroupUpdateDto>
{
    public Group[] Read(string[] names);
    public Group[] Read(Guid[] ids);
}