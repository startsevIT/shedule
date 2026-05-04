namespace Domain.Entity;

public interface ICrudRepository<T, in TUpdateDto> : IRepository<T>
    where T : ModifyEntity<TUpdateDto>
    where TUpdateDto : UpdateDto
{
    void Update(Guid id, TUpdateDto dto);
}

public abstract record UpdateDto;