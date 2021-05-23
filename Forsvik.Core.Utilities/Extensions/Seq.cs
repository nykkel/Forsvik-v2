using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Forsvik.Core.Utilities.Extensions
{
    /// <summary>
    /// Provides a functional factory
    /// </summary>
    public static class Seq
    {
        public static void Repeat(this int count, Action<int> run)
        {
            for (int i = 0; i < count; i++)
            {
                run(i);
            }
        }

        public static IEnumerable<T> Generate<T>(int count, Func<T> generator)
        {
            while (count-- > 0)
                yield return generator();
        }

        public static IEnumerable<int> OfInts(int count)
        {
            for (int i = 0; i < count; i++)
                yield return i;
        }

        public static IEnumerable<int> OfInts(int from, int to)
        {
            if (from < to)
                for (int i = from; i <= to; i++) yield return i;
            else
                for (int i = from; i >= to; i--) yield return i;
        }

        public static IEnumerable<T> One<T>(T one)
        {
            yield return one;
        }

        public static IEnumerable<T> Two<T>(T one, T two)
        {
            yield return one;
            yield return two;
        }

        public static IEnumerable<T> Three<T>(T one, T two, T three)
        {
            yield return one;
            yield return two;
            yield return three;
        }

        public static IEnumerable<T> Many<T>(params T[] arguments)
        {
            return arguments;
        }

        public static T[] Array<T>(params T[] arguments)
        {
            return arguments;
        }

        public static IEnumerable<T> Empty<T>()
        {
            yield break;
        }

        public static IEnumerable<T> Concat<T>(T first, IEnumerable<T> seq)
        {
            yield return first;
            foreach (var item in seq)
                yield return item;
        }

        public static IEnumerable<T> Concat<T>(IEnumerable<T> seq, T last)
        {
            foreach (var item in seq)
                yield return item;

            yield return last;
        }

        public static IEnumerable<T> Concat<T>(IEnumerable<T> firsts, IEnumerable<T> lasts)
        {
            foreach (var first in firsts)
                yield return first;

            foreach (var last in lasts)
                yield return last;
        }

        public static IEnumerable<T> Concat<T>(IEnumerable<IEnumerable<T>> sequences)
        {
            foreach (var sequence in sequences)
                foreach (var item in sequence)
                    yield return item;
        }

        public static IEnumerable<TResult> Map<T1, T2, TResult>(IEnumerable<T1> seqOne, IEnumerable<T2> seqTwo,
                                                                Func<T1, T2, TResult> function)
        {
            var enumOne = seqOne.GetEnumerator();
            var enumTwo = seqTwo.GetEnumerator();

            while (enumOne.MoveNext() && enumTwo.MoveNext())
                yield return function(enumOne.Current, enumTwo.Current);
        }

        public static void Do<T>(IEnumerable<T> seq, Action<T> action)
        {
            foreach (var item in seq)
                action(item);
        }

        public static void Do<TOne, TTwo>(IEnumerable<TOne> seqOne, IEnumerable<TTwo> seqTwo, Action<TOne, TTwo> action)
        {
            var enumOne = seqOne.GetEnumerator();
            var enumTwo = seqTwo.GetEnumerator();

            while (enumOne.MoveNext() && enumTwo.MoveNext())
                action(enumOne.Current, enumTwo.Current);
        }

        /// <summary>
        /// There is often a problem removing items from a collection since you mess up the iterator once one item is removed.
        /// This method solves this when working with a List with a predicate
        /// </summary>
        /// <typeparam name="T">the type of the sequence</typeparam>
        /// <param name="sequence">the original sequence</param>
        /// <param name="predicate">the method determining if the item should be removed or not</param>
        public static void Remove<T>(this List<T> sequence, Func<T, bool> predicate)
        {
            var itemsToRemove = sequence.Where(predicate);
            sequence.Remove(itemsToRemove);
        }

        /// <summary>
        /// There is often a problem removing items from a collection since you mess up the iterator once one item is removed.
        /// This method solves this when working with List
        /// </summary>
        /// <typeparam name="T">the type of the sequence</typeparam>
        /// <param name="sequence">the original sequence</param>
        /// <param name="itemsToRemove">the items in the original sequence to remove</param>
        public static void Remove<T>(this List<T> sequence, IEnumerable<T> itemsToRemove)
        {
            foreach (var item in itemsToRemove)
                sequence.Remove(item);
        }

        public static bool DoWhile<TOne, TTwo>(IEnumerable<TOne> seqOne, IEnumerable<TTwo> seqTwo,
                                               Func<TOne, TTwo, bool> predicateAction)
        {
            var enumOne = seqOne.GetEnumerator();
            var enumTwo = seqTwo.GetEnumerator();

            while (true)
            {
                var enumOneState = enumOne.MoveNext();
                var enumTwoState = enumTwo.MoveNext();

                if (enumOneState != enumTwoState)
                    return false;

                if (!enumOneState && !enumTwoState)
                    return true;

                if (!predicateAction(enumOne.Current, enumTwo.Current))
                    return false;
            }
        }

        public static bool DoUntil<TOne, TTwo>(IEnumerable<TOne> seqOne, IEnumerable<TTwo> seqTwo,
                                               Func<TOne, TTwo, bool> predicateAction)
        {
            var enumOne = seqOne.GetEnumerator();
            var enumTwo = seqTwo.GetEnumerator();

            while (true)
            {
                var oneHasItems = enumOne.MoveNext();
                var twoHasItems = enumTwo.MoveNext();

                if (oneHasItems != twoHasItems)
                    return false;

                if (!oneHasItems && !twoHasItems)
                    return true;

                if (predicateAction(enumOne.Current, enumTwo.Current))
                    return false;
            }
        }

        public static IEnumerable<T> Make<T>(int numberOfItems) where T : new()
        {
            for (int i = 0; i < numberOfItems; i++)
                yield return new T();
        }

        public static T Make<T>() where T : new()
        {
            return Make<T>(1).Single();
        }

        public static bool AreEqual<T>(IEnumerable<T> first, IEnumerable<T> second, Func<T, T, bool> comparer)
        {
            if (first == null || second == null)
                return false;

            var firstEnumerator = first.GetEnumerator();
            var secondEnumerator = second.GetEnumerator();
            bool firstHasMoreElements;
            bool secondHasMoreElements;

            while (true)
            {
                firstHasMoreElements = firstEnumerator.MoveNext();
                secondHasMoreElements = secondEnumerator.MoveNext();

                if (!(firstHasMoreElements && secondHasMoreElements))
                    break;

                if (comparer(firstEnumerator.Current, secondEnumerator.Current) == false)
                    return false;
            }

            return !firstHasMoreElements && !secondHasMoreElements;
        }
    }
}
