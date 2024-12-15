using System.Linq.Expressions;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class TematskoPodrucjeSort
    {
        public static IQueryable<TematskoPodrucje> ApplySort(this IQueryable<TematskoPodrucje> query, int sort, bool ascending)
        {
            Expression<Func<TematskoPodrucje, object>> orderSelector = sort switch
            {
                1 => d => d.IdTematskogPodrucja,
                2 => d => d.TematskoPodrucje1,
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
