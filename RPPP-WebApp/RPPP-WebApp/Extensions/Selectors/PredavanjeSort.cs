using System.Linq.Expressions;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class PredavanjeSort
    {
        public static IQueryable<Predavanje> ApplySort(this IQueryable<Predavanje> query, int sort, bool ascending)
        {
            Expression<Func<Predavanje, object>> orderSelector = sort switch
            {
                1 => d => d.VrijemePocetka,
                2 => d => d.VrijemeZavrsetka,
                3 => d => d.IdPredavanja,
                4 => d => d.IdRaspored,
                5 => d => d.SifPredmet,
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
