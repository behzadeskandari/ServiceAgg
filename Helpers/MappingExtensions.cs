using Microsoft.EntityFrameworkCore;
using ServiceAggregator.Api.Dto;
namespace ServiceAggregator.Api.Helpers;
public static class MappingExtensions
{
    public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(this IQueryable<TDestination> queryable,
        int pageIndex, int pageSize, CancellationToken cancellationToken) where TDestination : class
        => PaginatedList<TDestination>.CreateAsync(queryable.AsNoTracking(), pageIndex, pageSize, cancellationToken);

    public static Task<PaginatedList<TDestination>> ListAsync<TDestination>(this IQueryable<TDestination> queryable,
        CancellationToken cancellationToken) where TDestination : class
        => PaginatedList<TDestination>.CreateAsync(queryable.AsNoTracking(), cancellationToken);
}
