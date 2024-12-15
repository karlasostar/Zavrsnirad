using System.Linq.Expressions;
using RPPP_WebApp.Models;
namespace RPPP_WebApp.Extensions.Selectors
{
    public static class VrstaOdlukeSort
    {
        public static IQueryable<VrstaOdluke> ApplySort(this IQueryable<VrstaOdluke> query, int sort, bool ascending)
        {
            Expression<Func<VrstaOdluke, object>> orderSelector = sort switch
            {
                1 => d => d.IdVrstaOdluke,
                2 => d => d.VrstaOdluke1,
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
