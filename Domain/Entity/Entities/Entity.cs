namespace Domain.Entity.Entities;

public abstract class Entity
{
    public DateTime CreationTime { get; set; } = DateTime.Now;
    public Guid Id { get; private set; } = Guid.NewGuid();
}