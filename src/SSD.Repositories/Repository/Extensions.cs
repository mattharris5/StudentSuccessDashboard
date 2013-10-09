using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SSD.Repository
{
    public static class Extensions
    {
        public static IEnumerable<string> CompletionListFor<T>(this IQueryable<T> query, Expression<Func<T, CompletionProjection>> completionExpression, string term)
        {
            var filteredItems = FilterList(query, completionExpression, term).OrderBy(s => s).Take(10);
            return filteredItems.ToList();
        }

        public static IEnumerable<string> CompletionListFor<T>(this IQueryable<T> query, Expression<Func<T, CompletionProjection>> completionExpression, string term, IComparer<string> completionComparer)
        {
            var filteredItems = FilterList(query, completionExpression, term).ToList().OrderBy(s => s, completionComparer).Take(10);
            return filteredItems;
        }

        private static IQueryable<string> FilterList<T>(IQueryable<T> query, Expression<Func<T, CompletionProjection>> completionExpression, string term)
        {
            var items = query.Select(completionExpression);
            var newItems = items.Where(s => s.Value != null).Select(s => s.Value);
            if (query.Provider.GetType() == typeof(EnumerableQuery<T>).GetGenericTypeDefinition().MakeGenericType(typeof(T)))
            {
                newItems = newItems.Distinct(StringComparer.CurrentCultureIgnoreCase);
            }
            else
            {
                newItems = newItems.Distinct();
            }
            return newItems.Where(item => item.ToUpper().Contains(term.ToUpper()));
        }
    }
}
