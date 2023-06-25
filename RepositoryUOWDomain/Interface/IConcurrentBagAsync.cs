namespace RepositoryUOWDomain.Interface;

public interface IConcurrentBagAsync<in T>
{
    Task Add(T elem, CancellationToken token = default);
    Task AddRange(IEnumerable<T> elemCollect, CancellationToken token = default);
}