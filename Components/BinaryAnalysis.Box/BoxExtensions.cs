using System;
using System.Linq;
using BinaryAnalysis.Data.Box;

namespace BinaryAnalysis.Box
{
    public static class BoxExtensions
    {
        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> query, int index = 0, int pageSize = 10)
        {
            return new PagedList<T>(query, index, pageSize);
        }
        public static IBox CreateFor(Type type)
        {
            return Activator.CreateInstance(typeof (Box<>).MakeGenericType(type)) as IBox;
        }
    }
}