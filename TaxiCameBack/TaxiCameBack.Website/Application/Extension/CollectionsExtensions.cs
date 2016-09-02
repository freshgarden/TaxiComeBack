using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiCameBack.Website.Application.Extension
{
    public static class CollectionsExtensions
    {
        public static string ToConcatenatedString<T>(this IEnumerable<T> source, Func<T, string> toString, string separator)
        {
            if (source == null || !source.Any()) return string.Empty;

            var sb = new StringBuilder(toString(source.First()));
            foreach (var t in source.Skip(1))
            {
                sb.Append(separator);
                sb.Append(toString(t));
            }
            return sb.ToString();
        }


        public static string ToConcatenatedString<T>(this IEnumerable<T> list, string delimiter)
        {
            return list.ToConcatenatedString(t => t.ToString(), delimiter);
        }

        public static string ToConcatenatedString<T>(this IEnumerable<T> list)
        {
            return list.ToConcatenatedString(t => t.ToString(), ", ");
        }
    }
}