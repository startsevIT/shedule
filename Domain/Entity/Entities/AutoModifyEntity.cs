using System.Reflection;

namespace Domain.Entity.Entities;

public abstract class AutoModifyEntity<TUpdateDto> : ModifyEntity<TUpdateDto>
    where TUpdateDto : UpdateDto
{
    public override void Update(TUpdateDto dto) => GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite)
            .ToList()
            .ForEach(p => p.SetValue(this, p.GetValue(dto) ?? p.GetValue(this)));
}