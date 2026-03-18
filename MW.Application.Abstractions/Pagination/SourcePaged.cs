using Dr.Pagination;
using Microsoft.EntityFrameworkCore;

namespace MW.Application.Abstractions.Pagination
{
    /// <summary>
    /// Represents a paged data source with pagination information.
    /// </summary>
    /// <typeparam name="T">The type of elements in the data source.</typeparam>
    public class SourcePaged<T> : Paged<T>
    {
        
        /// <summary>
        /// Asynchronously retrieves a paged data source based on the provided request.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="source">The IQueryable data source to be paged.</param>
        /// <param name="req">The paging request information.</param>
        /// <returns>A SourcePaged instance containing the paged data and pagination information.</returns>
        public static async Task<SourcePaged<TEntity>> PagedAsync<TEntity>(IQueryable<TEntity> source, PageReq req)
            where TEntity : class
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            req.Page = req.Page == 0 ? 1 : req.Page;

            // PerPage Count
            if (req.PerPage > 0 && req.PerPage <= 200)
                Paginate<TEntity>.PerPage = req.PerPage;

            return new SourcePaged<TEntity>
            {
                PagingModel = new PageModel(await source.CountAsync(), req.Page, Paginate<TEntity>.PerPage),
                Source = await Paginate<TEntity>.Paging(source, req.Page).ToListAsync(),
            };
        }
        
        public static async Task<SourcePaged<TEntity>> PagedAsync<TEntity>(IQueryable<TEntity> source, PageReq req,Func<List<TEntity>,List<TEntity>> func)
            where TEntity : class
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            req.Page = req.Page == 0 ? 1 : req.Page;

            // PerPage Count
            if (req.PerPage > 0 && req.PerPage <= 200)
                Paginate<TEntity>.PerPage = req.PerPage;

            return new SourcePaged<TEntity>
            {
                PagingModel = new PageModel(await source.CountAsync(), req.Page, Paginate<TEntity>.PerPage),
                Source = func((await Paginate<TEntity>.Paging(source, req.Page).ToListAsync())),
            };
        }

        public static async Task<(IQueryable<T> pagedSource, PageModel paging)> PagedSourceQueryAsync(IQueryable<T> source, PageReq req)
        {
            
            if (source == null) throw new ArgumentNullException(nameof(source));
            req.Page = req.Page == 0 ? 1 : req.Page;

            // PerPage Count
            if (req.PerPage > 0 && req.PerPage <= 200)
                Paginate<T>.PerPage = req.PerPage;

            return (Paginate<T>.Paging(source, req.Page),
                new PageModel(await source.CountAsync(), req.Page, Paginate<T>.PerPage));
        }

        public static SourcePaged<T> Paged(List<T> source, PageModel pageModel) 
            => new() { Source = source, PagingModel = pageModel };

        public static async Task<(List<T> pagedSource, PageModel paging)> PagedSourceAsync(IQueryable<T> source, PageReq req)
        {
            var (paged, model) = await PagedSourceQueryAsync(source, req);
            return (await paged.ToListAsync(), model);
        }
    }

    public static class PagedUtils
    {
        
        public static SourcePaged<T> ToPaged<T>(this IQueryable<T> source, PageReq req)
            where T : class
        {
            return (SourcePaged<T>) SourcePaged<T>.SourcePaged(source, req);
        }
        
        public static Task<SourcePaged<T>> ToPagedAsync<T>(this IQueryable<T> source, PageReq req)
        where T : class
        {
            return SourcePaged<T>.PagedAsync(source, req);
        }
        
        public static Task<SourcePaged<T>> ToPagedAsync<T>(this IQueryable<T> source, PageReq req,Func<List<T>,List<T>> func)
            where T : class
        {
            return SourcePaged<T>.PagedAsync(source, req,func);
        }
    }

}

