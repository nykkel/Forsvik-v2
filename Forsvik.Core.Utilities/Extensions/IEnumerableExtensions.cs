using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Forsvik.Core.Utilities.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Do<T>(this IEnumerable<T> seq, Action<T> action)
        {
            Seq.Do(seq, action);
            return seq;
        }

        public static bool HasAny<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
                return false;

            return collection.Any();
        }

        public static void AddRange<T>(this IList<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
                collection.Add(item);
        }

        /// <summary>
        /// Will uniqueify a sequense where the property that should be unique is submitted as uniqueOn.
        /// The first instance of a duplicate item will be used. The rest ignored.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="seq"></param>
        /// <param name="uniqueOn"></param>
        /// <returns></returns>
        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> seq, Expression<Func<T, object>> uniqueOn)
        {
            var member = uniqueOn.Body as MemberExpression ?? ((UnaryExpression)uniqueOn.Body).Operand as MemberExpression;

            if (member == null)
                throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.", uniqueOn));

            var uniqueProp = member.Member as PropertyInfo;

            var list = new List<T>();
            var uniqueValues = new HashSet<object>();

            foreach (var item in seq)
            {
                var uniqueVal = uniqueProp.GetValue(item, null);
                if (!uniqueValues.Contains(uniqueVal))
                {
                    list.Add(item);
                    uniqueValues.Add(uniqueVal);
                }
            }

            return list;
        }

        public static IEnumerable<T> Slice<T>(this IEnumerable<T> seq, int startIndex, int count)
        {
            var index = 0;
            var counter = 0;
            foreach (var item in seq)
            {
                if (index >= startIndex)
                {
                    if (counter == count)
                        yield break;

                    yield return item;
                    counter++;
                }
                index++;
            }
        }

        public static int FindFirst<T>(this IEnumerable<T> seq, Func<T, bool> predicate)
        {
            var cnt = 0;
            foreach (var item in seq)
            {
                if (predicate(item))
                    return cnt;
                cnt++;
            }
            return -1;
        }

        public static IEnumerable<string> MatchRegEx(this IEnumerable<string> seq, string regEx)
        {
            var reg = new Regex(regEx);
            return seq.Where(item => reg.Match(item).Success);
        }
    }
}
