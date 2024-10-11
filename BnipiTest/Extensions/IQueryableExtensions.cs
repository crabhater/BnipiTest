using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace BnipiTest.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Include<T, TProperty>(this IQueryable<T> source,
            Expression<Func<T, TProperty>> path) where T : class
        {
            return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.Include(source, path);
        }

        public static IIncludableQueryable<T, TProperty> ThenInclude<T, TPreviousProperty, TProperty>(this IIncludableQueryable<T, IEnumerable<TPreviousProperty>> source,
            Expression<Func<TPreviousProperty, TProperty>> path) where T : class
        {
            return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ThenInclude(source, path);
        }
    }



}
