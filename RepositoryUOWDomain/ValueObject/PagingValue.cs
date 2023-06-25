
namespace RepositoryUOWDomain.ValueObject;

public class PagingValue<T>
{
    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }

    public int IndexFrom { get; set; }

    public IEnumerable<T>? Items { get; set; }

    public bool HasPreviousPage => PageIndex - IndexFrom > 0;

    public bool HasNextPage => PageIndex - IndexFrom + 1 < TotalPages;
}