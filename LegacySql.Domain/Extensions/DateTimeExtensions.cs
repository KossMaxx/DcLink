using System;

namespace LegacySql.Domain.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime RoundUpToMillisecond(this DateTime dt)
        {
            return new DateTime(dt.Year,dt.Month,dt.Day,dt.Hour,dt.Minute,dt.Second, dt.Millisecond - dt.Millisecond%100);
        }
    }
}
