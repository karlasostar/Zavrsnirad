using System.Linq.Expressions;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class PredmetSort
    {
        public static IQueryable<Predmet> ApplySort(this IQueryable<Predmet> query, int sort, bool ascending)
        {
            Expression<Func<Predmet, object>> orderSelector = sort switch
            {
                1 => d => d.SifPredmet,
                2 => d => d.Naziv,
                3 => d => d.PlanProgram,
                4 => d => d.Program,
                5 => d => d.JelIzboran,
                6 => d => d.Ects,
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
