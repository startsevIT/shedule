using Domain.Entity;
using Domain.Marks;

namespace Application.Repositories;

public class MarkRepository(IDbContext dbContext) : CrudRepository<Mark, MarkUpdateDto>(dbContext), IMarkRepository;