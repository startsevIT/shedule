using Domain.Entity;
using Domain.Pairs;
using Domain.Users;

namespace Application.Repositories;

public class PairRepository(IDbContext dbContext)
    : CrudRepository<Pair, PairUpdateDto>(dbContext), IPairRepository
{
    public Pair[] Read(IEnumerable<DateTime> dates) =>
        Storage
            .Where(x => dates.Contains(x.Date))
            .ToArray();

    public PairReadDto[] Read(GetPairModel model, string fullName) =>
        Storage
            .Where(x => x.Date >= model.Start && x.Date <= model.End)
            .Where(x => x.TeacherId == model.UserId)
            .OrderBy(x => x.Date)
            .Select(x => new PairReadDto(
                x.Discipline,
                fullName,
                x.CabinetNumber,
                x.Date,
                x.GroupId))
            .ToArray();

    public PairReadDto[] Read(GetPairModel model, Guid groupId, IUserRepository userRepo)
    {
        var pairs = Storage
            .Where(x => x.Date >= model.Start && x.Date <= model.End)
            .Where(x => x.GroupId == groupId)
            .OrderBy(x => x.Date)
            .ToArray();

        var teachers = userRepo
            .Read(pairs.Select(x => x.TeacherId).ToArray())
            .ToDictionary(x => x.Id, x => x.FullName);

        return pairs.Select(p => new PairReadDto(
            p.Discipline,
            teachers[p.TeacherId],
            p.CabinetNumber,
            p.Date,
            p.GroupId)).ToArray();
    }


    public void Create(Pair[] pairs)
    {
        Storage.AddRange(pairs);
        SaveChanges();
    }

}