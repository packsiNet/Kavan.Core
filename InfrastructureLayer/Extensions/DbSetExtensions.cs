using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InfrastructureLayer.Extensions;

public static class DbSetExtensions
{
    // بازگرداندن IQueryable<T> که شرط CryptocurrencyId IN و OpenTime in list را اعمال می‌کند
    public static IQueryable<T> WhereDynamicOpenTimeAndCrypto<T>(this DbSet<T> dbSet, int cryptoId, List<DateTime> openTimes) where T : class
    {
        var param = Expression.Parameter(typeof(T), "e");
        var propCrypto = Expression.PropertyOrField(param, "CryptocurrencyId");
        var propOpenTime = Expression.PropertyOrField(param, "OpenTime");

        // e => e.CryptocurrencyId == cryptoId
        var left = Expression.Equal(propCrypto, Expression.Constant(cryptoId));

        // build openTimes.Contains(e.OpenTime)
        var containsMethod = typeof(List<DateTime>).GetMethod("Contains", new[] { typeof(DateTime) })!;
        var openTimesExpr = Expression.Constant(openTimes);
        var containsCall = Expression.Call(openTimesExpr, containsMethod, propOpenTime);

        var andExpr = Expression.AndAlso(left, containsCall);

        var lambda = Expression.Lambda<Func<T, bool>>(andExpr, param);
        return dbSet.Where(lambda);
    }

    // انتخاب فقط OpenTime از موجودیت (برای تبدیل به List<DateTime>)
    public static IQueryable<DateTime> SelectDynamicOpenTime<T>(this IQueryable<T> query) where T : class
    {
        var param = Expression.Parameter(typeof(T), "e");
        var propOpenTime = Expression.PropertyOrField(param, "OpenTime");
        var lambda = Expression.Lambda<Func<T, DateTime>>(propOpenTime, param);
        return query.Select(lambda);
    }
}