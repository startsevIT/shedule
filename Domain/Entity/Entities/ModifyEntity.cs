namespace Domain.Entity.Entities;

public abstract class ModifyEntity<TUpdateDto> : Entity
    where TUpdateDto : UpdateDto
{
    public DateTime? ModificationTime { get; set; }
    public abstract void Update(TUpdateDto dto);
}