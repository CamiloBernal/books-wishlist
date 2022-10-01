public interface IRepository<T> where T : class, new()
{
    Task<T> Create(T entity);
    Task<IEnumerable<T>> Read();

    Task<T> Update(T entity);

    Task<T> Delete(T entity);
}
