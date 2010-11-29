﻿#region License and Terms
//
// MoreLINQ - Extensions to LINQ to Objects
// Copyright (c) 2008-9 Jonathan Skeet. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System;
using System.Collections.Generic;

namespace MoreLinq
{
    public static partial class MoreEnumerable
    {
        /// <summary>
        /// Peforms a scan (inclusive prefix sum) on a sequence of elements.
        /// </summary>
        /// <remarks>
        /// An inclusive prefix sum returns an equal-length sequence where the
        /// N-th element is the sum of the first N input elements. More
        /// generally, the scan allows any commutative binary operation, not
        /// just a sum.
        /// The exclusive version of Scan is <see cref="PreScan"/>.
        /// This operator uses deferred execution and streams its result.
        /// </remarks>
        /// <example>
        /// <code>
        /// Func&lt;int, int, int&gt; plus = (a, b) =&gt; a + b;
        /// int[] values = { 1, 2, 3, 4 };
        /// IEnumerable&lt;int&gt; prescan = values.PreScan(plus, 0);
        /// IEnumerable&lt;int&gt; scan = values.Scan(plus; a + b);
        /// IEnumerable&lt;int&gt; result = values.Zip(prescan, plus);
        /// </code>
        /// <c>prescan</c> will yield <c>{ 0, 1, 3, 6 }</c>, while <c>scan</c>
        /// and <c>result</c> will both yield <c>{ 1, 3, 6, 10 }</c>. This
        /// shows the relationship between the inclusive and exclusive prefix sum.
        /// </example>
        /// <typeparam name="TSource">Type of elements in source sequence</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="transformation">Transformation operation</param>
        /// <returns>The scanned sequence</returns>
        /// <exception cref="System.InvalidOperationException">If <paramref name="source"/> is empty.</exception>
        
        public static IEnumerable<TSource> Scan<TSource>(this IEnumerable<TSource> source,
            Func<TSource, TSource, TSource> transformation)
        {
            source.ThrowIfNull("source");
            transformation.ThrowIfNull("transformation");
            return ScanImpl(source, transformation);
        }

        private static IEnumerable<T> ScanImpl<T>(IEnumerable<T> source, Func<T, T, T> f)
        {
            using (var i = source.GetEnumerator())
            {
                if (!i.MoveNext())
                {
                    throw new InvalidOperationException("source must not be empty");
                }
                var aggregator = i.Current;

                while (i.MoveNext())
                {
                    yield return aggregator;
                    aggregator = f(aggregator, i.Current);
                }
                yield return aggregator;
            }
        }
    }
}
