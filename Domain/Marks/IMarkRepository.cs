using Domain.Entity;

namespace Domain.Marks;

public interface IMarkRepository : IRepositoryForModifiable<Mark, MarkUpdateDto>;