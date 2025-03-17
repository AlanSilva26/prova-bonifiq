namespace ProvaPub.Models;

public class PaginatedList<T> where T : class
{
    public PaginatedList(List<T> items, int totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }

    public List<T> Items { get; set; }

    public int TotalCount { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public bool HasNext => (Page * PageSize) < TotalCount;
}
