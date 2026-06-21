using Domain.Entity;
using Domain.Groups;
using Domain.Pairs;
using Domain.Users;

namespace Application.Repositories;

public class PairRepository(
    IDbContext dbContext,
    IUserRepository userRepo,
    IGroupRepository groupRepo)
    : CrudRepository<Pair, PairUpdateDto>(dbContext), IPairRepository
{
    public Pair[] Read(IEnumerable<DateTime> dates) =>
        Storage
            .Where(x => dates.Contains(x.Date))
            .ToArray();

    public PairReadDto[] ReadForTeacher(GetPairModel model)
    {
        var teacher = userRepo
            .Read(model.UserId!.Value);

        var pairs = Storage
            .Where(x => x.Date >= model.Start && x.Date <= model.End)
            .Where(x => x.TeacherId == model.UserId).ToArray();

        var groups = groupRepo
            .Read(pairs.Select(x => x.GroupId).ToArray())
            .ToDictionary(x => x.Id, x => x.Name);

        return pairs
            .OrderBy(x => x.Date)
            .Select(x => new PairReadDto(
                x.Discipline,
                teacher!.FullName,
                x.CabinetNumber,
                groups[x.GroupId],
                x.Date,
                x.Color))
            .ToArray();
    }


    public PairReadDto[] Read(GetPairModel model, Guid groupId)
    {
        var pairs = Storage
            .Where(x => x.Date >= model.Start && x.Date <= model.End)
            .Where(x => x.GroupId == groupId)
            .OrderBy(x => x.Date)
            .ToArray();

        var teachers = userRepo
            .Read(pairs.Select(x => x.TeacherId).ToArray())
            .ToDictionary(x => x.Id, x => x.FullName);

        var groups = groupRepo
                .Read(pairs.Select(x => x.GroupId).ToArray())
                .ToDictionary(x => x.Id, x => x.Name);

        return pairs.Select(p => new PairReadDto(
            p.Discipline,
            teachers[p.TeacherId],
            p.CabinetNumber,
            groups[p.GroupId],
            p.Date,
            p.Color)).ToArray();
    }


    public void Create(Pair[] pairs)
    {
        Storage.AddRange(pairs);
        SaveChanges();
    }

}