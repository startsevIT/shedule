using Domain.Entity;
using Domain.Users;

namespace Domain.Pairs;

public interface IPairRepository : ICrudModifiable<Pair, PairUpdateDto>
{
    void Create(Pair[] toArray);
    public Pair[] Read(IEnumerable<DateTime> dates);
    public PairReadDto[] Read(GetPairModel model, string fullName);
    public PairReadDto[] Read(GetPairModel model, Guid groupId, IUserRepository userRepo);
}

public record PairReadDto(
    string Discipline,
    string Teacher,
    string CabinetNumber,
    DateTime Date,
    Guid GroupId);

public record GetPairModel(
    DateTime Start,
    DateTime End,
    Guid? UserId = null);