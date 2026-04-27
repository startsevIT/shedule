namespace Domain.Entity;

public interface IRepository<T> where T : Entity
{
    void Create(T entity);
    T? Read(Guid id);
    void Delete(Guid id);
}