using Microsoft.EntityFrameworkCore;
namespace ServiceAggregator.Api.Dto;
public class PaginatedList<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }

    public PaginatedList(IReadOnlyCollection<T> items, int count, int pageIndex, int pageSize)
    {
        PageNumber = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items;
    }

    public PaginatedList(IReadOnlyCollection<T> items, int count)
    {
        TotalCount = count;
        Items = items;
    }

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize,
        CancellationToken cancellationToken)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, CancellationToken cancellationToken)
    {
        var count = await source.CountAsync();
        var items = await source.ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, count);
    }
}
