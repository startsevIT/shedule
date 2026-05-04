using Domain.Entity;

namespace Domain.Marks;

public interface IMarkRepository : ICrudRepository<Mark, MarkUpdateDto>;