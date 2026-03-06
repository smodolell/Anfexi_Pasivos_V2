using Anfx.Pasivos.Application.Common.DTOs;
using Anfx.Pasivos.Application.Common.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Anfx.Pasivos.Infrastructure.Services;

public sealed class Paginator : IPaginator
{
    public Paginator()
    {
        // Constructor vacío
    }

    async Task<PagedResultDto<TDestination>> IPaginator.PaginateAsync<T, TDestination>(
        IQueryable<T> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ProjectToType<TDestination>()
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(total / (double)pageSize);

        var pagedResult = new PagedResultDto<TDestination>
        {
            Results = items,  // items es List<TDestination>, que implementa IEnumerable<TDestination>
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = totalPages
        };

        return pagedResult;
    }

    public PagedResultDto<T> CreatePagedResult<T>(
        List<T> items,
        int pageNumber,
        int pageSize,
        long totalCount)
        where T : class, new()
    {
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResultDto<T>
        {
            Results = items,  
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalCount = (int)totalCount,
            TotalPages = totalPages
        };
    }
}