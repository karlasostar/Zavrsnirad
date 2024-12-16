using System.Linq.Expressions;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.Extensions.Selectors
{
	public static class RasporedSort
	{
		public static IQueryable<Raspored> ApplySort(this IQueryable<Raspored> query, int sort, bool ascending)
		{
			Expression<Func<Raspored, object>> orderSelector = sort switch
			{
				1 => d => d.IdRaspored,
				2 => d => d.Opis,
				3 => d => d.IdAkGod,
				4 => d => d.IdDvorana,
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
