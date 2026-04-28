namespace Domain.Entity;

public abstract class CrudRepository<T, TUpdateDto>(IDbContext dbContext)
    : CrdRepository<T>(dbContext), ICrudModifiable<T, TUpdateDto>
    where TUpdateDto : UpdateDto
    where T : ModifyEntity<TUpdateDto>
{
    public void Update(Guid id, TUpdateDto dto)
    {
        var result = Read(id);
        if (result == null)
            throw new ArgumentException($"The specified id '{id}' was not found.");
        result.ModificationTime = DateTime.Now;
        result.Update(dto);
        SaveChanges();
    }
}