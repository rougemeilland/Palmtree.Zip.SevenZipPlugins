using System;

namespace Palmtree.IO.Compression.Stream.Plugin.SevenZip
{
    public static class ProgressExtensions
    {
        private class ProgressReporter
            : IProgress<(UInt64? inStreamProcessedCount, UInt64? outStreamProcessedCount)>
        {
            private readonly IProgress<UInt64> _sourceProgress;

            public ProgressReporter(IProgress<UInt64> sourceProgress)
            {
                _sourceProgress = sourceProgress;
            }

            void IProgress<(UInt64? inStreamProcessedCount, UInt64? outStreamProcessedCount)>.Report((UInt64? inStreamProcessedCount, UInt64? outStreamProcessedCount) value)
            {
                if (value.inStreamProcessedCount is not null)
                    _sourceProgress.Report(value.inStreamProcessedCount.Value);
            }
        }

        public static IProgress<(UInt64? inStreamProcessedCount, UInt64? outStreamProcessedCount)>? ToInStremProgress(this IProgress<UInt64>? progress)
        {
            if (progress is null)
                return null;

            return new ProgressReporter(progress);
        }
    }
}
