using Domain.Entity;

namespace Domain.Marks;

public interface IMarkRepository : ICrudModifiable<Mark, MarkUpdateDto>;