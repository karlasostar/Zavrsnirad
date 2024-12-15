using System.Linq.Expressions;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class DvoranaSort
    {
        public static IQueryable<Dvorana> ApplySort(this IQueryable<Dvorana> query, int sort, bool ascending)
        {
            Expression<Func<Dvorana, object>> orderSelector = sort switch
            {
                1 => d => d.OznDvorana,
                2 => d => d.IdDvorana,
                3 => d => d.Kapacitet,
                _ => null
            };

            if (orderSelector != null)
            {
                query = ascending ?
                       query.OrderBy(orderSelector) :
                       query.OrderByDescending(orderSelector);
            }

            return query;
        }
    }
}
