using System;
using SevenZip.Compression.Bzip2;

namespace Palmtree.IO.Compression.Stream.Plugin.SevenZip
{
    internal sealed class Bzip2DecoderPlugin
        : ICompressionCoder, ICompressionHierarchicalDecoder
    {
        private sealed class Decoder
            : HierarchicalDecoder
        {
            private Decoder(
                ISequentialInputByteStream baseStream,
                UInt64 unpackedStreamSize,
                IProgress<(UInt64 inCompressedStreamProcessedCount, UInt64 outUncompressedStreamProcessedCount)>? progress,
                Boolean leaveOpen,
                Func<ISequentialInputByteStream, ISequentialInputByteStream> decoderStreamCreator)
                : base(baseStream, unpackedStreamSize, progress, leaveOpen, decoderStreamCreator)
            {
            }

            public static Decoder Create(
                ISequentialInputByteStream baseStream,
                UInt64 unpackedStreamSize,
                IProgress<(UInt64 inCompressedStreamProcessedCount, UInt64 outUncompressedStreamProcessedCount)>? progress,
                Boolean leaveOpen)
                => new(
                    baseStream,
                    unpackedStreamSize,
                    progress,
                    leaveOpen,
                    stream => Bzip2Decoder.CreateDecoderStream(stream, unpackedStreamSize));
        }

        CompressionMethodId ICompressionCoder.CompressionMethodId => Bzip2CoderPlugin.COMPRESSION_METHOD_ID;

        ISequentialInputByteStream IHierarchicalDecoder.CreateDecoderStream(
            ISequentialInputByteStream baseStream,
            ICoderOption option,
            UInt64 unpackedStreamSize,
            UInt64 packedStreamSize,
            IProgress<(UInt64 inCompressedStreamProcessedCount, UInt64 outUncompressedStreamProcessedCount)>? progress,
            Boolean leaveOpen)
        {
            ArgumentNullException.ThrowIfNull(baseStream);
            ArgumentNullException.ThrowIfNull(option);
            if (option is not ZipBzip2CompressionCoderOption)
                throw new ArgumentException($"Illegal {nameof(option)} data", nameof(option));

            return Decoder.Create(baseStream, unpackedStreamSize, progress, leaveOpen);
        }
    }
}
