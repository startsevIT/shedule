using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity;

public abstract class ModifyEntity<TUpdateDto> : Entity
    where TUpdateDto : UpdateDto
{
    [Column("modification_time")]
    public DateTime? ModificationTime { get; set; }
    public abstract void Update(TUpdateDto dto);
}