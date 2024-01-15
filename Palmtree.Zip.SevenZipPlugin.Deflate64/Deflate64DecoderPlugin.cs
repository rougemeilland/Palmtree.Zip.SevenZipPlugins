using System;
using SevenZip.Compression.Deflate64;

namespace Palmtree.IO.Compression.Stream.Plugin.SevenZip
{
    internal class Deflate64DecoderPlugin
        : ICompressionCoder, ICompressionHierarchicalDecoder
    {
        private class Decoder
            : HierarchicalDecoder
        {
            public Decoder(
                ISequentialInputByteStream baseStream,
                UInt64 unpackedStreamSize,
                IProgress<(UInt64 inCompressedStreamProcessedCount, UInt64 outUncompressedStreamProcessedCount)>? progress,
                Boolean leaveOpen,
                Func<ISequentialInputByteStream, ISequentialInputByteStream> decoderStreamCreator)
                : base(baseStream, unpackedStreamSize, progress, leaveOpen, decoderStreamCreator)
            {
            }

            public static ISequentialInputByteStream Create(
                ISequentialInputByteStream baseStream,
                UInt64 unpackedStreamSize,
                IProgress<(UInt64 inCompressedStreamProcessedCount, UInt64 outUncompressedStreamProcessedCount)>? progress,
                Boolean leaveOpen)
                => new Decoder(
                    baseStream,
                    unpackedStreamSize,
                    progress,
                    leaveOpen,
                    stream => Deflate64Decoder.CreateDecoderStream(stream, unpackedStreamSize));
        }

        CompressionMethodId ICompressionCoder.CompressionMethodId => Deflate64CoderPlugin.COMPRESSION_METHOD_ID;

        ISequentialInputByteStream IHierarchicalDecoder.CreateDecoderStream(
            ISequentialInputByteStream baseStream,
            ICoderOption option,
            UInt64 unpackedStreamSize,
            UInt64 packedStreamSize,
            IProgress<(UInt64 inCompressedStreamProcessedCount, UInt64 outUncompressedStreamProcessedCount)>? progress,
            Boolean leaveOpen)
        {
            if (baseStream is null)
                throw new ArgumentNullException(nameof(baseStream));
            if (option is null)
                throw new ArgumentNullException(nameof(option));
            if (option is not ZipDeflate64CompressionCoderOption)
                throw new ArgumentException($"Illegal {nameof(option)} data", nameof(option));

            return Decoder.Create(baseStream, unpackedStreamSize, progress, leaveOpen);
        }
    }
}
