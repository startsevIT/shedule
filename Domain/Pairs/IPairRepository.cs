using Domain.Entity;

namespace Domain.Pairs;

public interface IPairRepository : ICrudRepository<Pair, PairUpdateDto>
{
    void Create(Pair[] toArray);
    public Pair[] Read(IEnumerable<DateTime> dates);
    public PairReadDto[] ReadForTeacher(GetPairModel model);
    PairReadDto[] Read(GetPairModel model, Guid groupId);

}

public record PairReadDto(
    string Discipline,
    string Teacher,
    string Cabinet,
    string Group,
    DateTime Date,
    string Color);

public record GetPairModel(
    DateTime Start,
    DateTime End,
    Guid? UserId = null);