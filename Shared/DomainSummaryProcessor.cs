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
    }
}
