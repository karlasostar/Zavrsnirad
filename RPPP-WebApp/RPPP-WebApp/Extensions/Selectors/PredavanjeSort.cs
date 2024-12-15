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
                1 => d => d.IdPredavanja,
                2 => d => d.SifPredmetNavigation.Naziv,
                3 => d => d.VrijemePocetka,
                4 => d => d.VrijemeZavrsetka,
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
