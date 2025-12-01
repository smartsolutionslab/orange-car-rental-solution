namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

/// <summary>
///     Extension methods for IEnumerable to support pagination and other common query operations.
///     Uses C# 14 Extension Members syntax.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    ///     C# 14 Extension Members for IEnumerable pagination.
    /// </summary>
    extension<TItem>(IEnumerable<TItem> source)
    {
        /// <summary>
        ///     Applies pagination to an enumerable (in-memory collection) and returns a paged result.
        /// </summary>
        public PagedResult<TItem> ToPagedResult(SearchParameters parameters)
        {
            var paging = parameters.Paging;

            var items = source as IList<TItem> ?? source.ToList();

            var pagedItems = items
                .Skip(paging.Skip)
                .Take(paging.Take)
                .ToList();

            return new PagedResult<TItem>
            {
                Items = pagedItems,
                TotalCount = items.Count,
                PageNumber = paging.PageNumber,
                PageSize = paging.PageSize
            };
        }
    }
}
