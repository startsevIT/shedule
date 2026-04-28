using Domain.Entity;

namespace Domain.Groups;

public interface IGroupRepository : ICrudModifiable<Group, GroupUpdateDto>
{
    public Group[] Read(string[] names);
}