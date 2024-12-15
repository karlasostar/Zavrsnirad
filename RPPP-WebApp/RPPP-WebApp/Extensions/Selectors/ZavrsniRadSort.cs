//using RPPP_WebApp.Models;
//using System.Linq.Expressions;

//namespace RPPP_WebApp.Extensions.Selectors
//{
//    public static class ZavrsniRadSort
//    {
//        public static IQueryable<ZavrsniRad> ApplySort(this IQueryable<ZavrsniRad> query, int sort, bool ascending)
//        {
//            Expression<Func<ZavrsniRad, object>> orderSelector = sort switch
//            {
//                1 => d => d.IdRad,
//                2 => d => d.Naslov,
//                3 => d => d.Metodologija,
//                4 => d => d.Ocjena,
//                5 => d => d.Tema,
//                6 => d => d.Opis,
//                7 => d => d.Sazetak,
//                8 => d => d.DatumObrane,
//                9 => d => d.IdTematskogPodrucjaNavigation.IdTematskogPodrucja,
//                10 => d => d.IdVijecaNavigation.IdVijeca,
//                11 => d => d.Student.Oib,
//                _ => null
//            };

//            if (orderSelector != null)
//            {
//                query = ascending ?
//                       query.OrderBy(orderSelector) :
//                       query.OrderByDescending(orderSelector);
//            }

//            return query;
//        }
//    }
//}

using RPPP_WebApp.Models;
using System.Linq.Expressions;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class ZavrsniRadSort
    {
        public static IQueryable<ZavrsniRad> ApplySort(this IQueryable<ZavrsniRad> query, int sort, bool ascending)
        {
            Expression<Func<ZavrsniRad, object>> orderSelector = sort switch
            {
                1 => d => d.IdRad,
                2 => d => d.Naslov,
                3 => d => d.Metodologija,
                4 => d => d.Ocjena,
                5 => d => d.Tema,
                6 => d => d.Opis,
                7 => d => d.Sazetak,
                8 => d => d.DatumObrane,
                9 => d => d.IdTematskogPodrucjaNavigation.IdTematskogPodrucja,
                10 => d => d.IdVijecaNavigation.IdVijeca,
                11 => d => d.Student.Oib,
                _ => null
            };

            if (orderSelector != null)
            {
                query = ascending ? query.OrderBy(orderSelector) : query.OrderByDescending(orderSelector);
            }

            return query;
        }
    }
}
