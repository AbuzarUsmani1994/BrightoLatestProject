using System;
using System.Collections.Generic;
using System.Linq;

namespace FOS.Shared
{
    public class DomainSummary
    {
        public int Total { get; set; }
        public Dictionary<string, int> ByGroup { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> Sums { get; set; } = new Dictionary<string, decimal>();
        public Dictionary<string, DomainSummary> BySO { get; set; } = new Dictionary<string, DomainSummary>();
    }

    public static class DomainSummaryProcessor
    {
        public static DomainSummary Summarize<T>(
            IEnumerable<T> rows,
            Func<T, string> groupKeySelector = null,
            Dictionary<string, Func<T, decimal?>> sumSelectors = null)
        {
            var list = rows as IList<T> ?? rows.ToList();
            var summary = new DomainSummary { Total = list.Count };

            if (groupKeySelector != null)
            {
                summary.ByGroup = list
                    .GroupBy(r => string.IsNullOrEmpty(groupKeySelector(r)) ? "Unknown" : groupKeySelector(r))
                    .ToDictionary(g => g.Key, g => g.Count());
            }

            if (sumSelectors != null)
            {
                foreach (var kv in sumSelectors)
                {
                    summary.Sums[kv.Key] = list.Sum(r => kv.Value(r) ?? 0m);
                }
            }

            return summary;
        }

        // Same as Summarize, plus a per-SO breakdown (each SO's own Total/ByGroup/Sums)
        // under the returned summary's BySO dictionary, keyed by SO name.
        public static DomainSummary SummarizeBySO<T>(
            IEnumerable<T> rows,
            Func<T, string> soNameSelector,
            Func<T, string> groupKeySelector = null,
            Dictionary<string, Func<T, decimal?>> sumSelectors = null)
        {
            var list = rows as IList<T> ?? rows.ToList();
            var overall = Summarize(list, groupKeySelector, sumSelectors);

            overall.BySO = list
                .GroupBy(r => string.IsNullOrEmpty(soNameSelector(r)) ? "Unknown" : soNameSelector(r))
                .ToDictionary(g => g.Key, g => Summarize(g.ToList(), groupKeySelector, sumSelectors));

            return overall;
        }
    }
}
