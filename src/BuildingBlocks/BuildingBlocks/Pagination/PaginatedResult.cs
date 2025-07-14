namespace BuildingBlocks.Pagination
{
    public class PaginatedResult<TEntity>
        (int pageIndex, int pageSize, long totalItems, IEnumerable<TEntity> data) where TEntity : class
    {
        public int PageIndex { get; } = pageIndex;
        public int PageSize { get; } = pageSize;
        public long TotalItems { get; } = totalItems;
        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
        public IEnumerable<TEntity> Data { get; } = data;
    }
}
