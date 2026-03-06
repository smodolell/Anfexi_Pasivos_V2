namespace Anfx.Pasivos.Application.Common.Interfaces;

public interface IDynamicSorter
{
    IQueryable<T> ApplySort<T>(
        IQueryable<T> query,
        string sortColumn,
        bool descending = false);

    IQueryable<T> ApplySort<T>(
            IQueryable<T> query,
            Dictionary<string, bool> sortColumns);
}
