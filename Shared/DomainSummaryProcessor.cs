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
        // The relevant record's date/time for this group, when a timestampSelector is
        // given (e.g. an SO's attendance check-in time) - the latest one if several rows
        // fall into the same group.
        public DateTime? Timestamp { get; set; }
    }

    public static class DomainSummaryProcessor
    {
        public static DomainSummary Summarize<T>(
            IEnumerable<T> rows,
            Func<T, string> groupKeySelector = null,
            Dictionary<string, Func<T, decimal?>> sumSelectors = null,
            Func<T, DateTime?> timestampSelector = null)
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

            if (timestampSelector != null)
            {
                var timestamps = list.Select(timestampSelector).Where(t => t.HasValue).Select(t => t.Value).ToList();
                if (timestamps.Count > 0) summary.Timestamp = timestamps.Max();
            }

            return summary;
        }

        // Same as Summarize, plus a per-SO breakdown (each SO's own Total/ByGroup/Sums/Timestamp)
        // under the returned summary's BySO dictionary, keyed by SO name.
        public static DomainSummary SummarizeBySO<T>(
            IEnumerable<T> rows,
            Func<T, string> soNameSelector,
            Func<T, string> groupKeySelector = null,
            Dictionary<string, Func<T, decimal?>> sumSelectors = null,
            Func<T, DateTime?> timestampSelector = null)
        {
            var list = rows as IList<T> ?? rows.ToList();
            var overall = Summarize(list, groupKeySelector, sumSelectors, timestampSelector);

            overall.BySO = list
                .GroupBy(r => string.IsNullOrEmpty(soNameSelector(r)) ? "Unknown" : soNameSelector(r))
                .ToDictionary(g => g.Key, g => Summarize(g.ToList(), groupKeySelector, sumSelectors, timestampSelector));

            return overall;
        }
    }
}
