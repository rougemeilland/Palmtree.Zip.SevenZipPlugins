﻿using System;
using System.IO;
using SevenZip.Compression.Lzma;

namespace Palmtree.IO.Compression.Stream.Plugin.SevenZip
{
    internal sealed class LzmaDecoderPlugin
        : ICompressionCoder, ICompressionHierarchicalDecoder
    {
        private sealed class Decoder
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

            public static Decoder Create(
                ISequentialInputByteStream baseStream,
                UInt64 unpackedStreamSize,
                IProgress<(UInt64 inCompressedStreamProcessedCount, UInt64 outUncompressedStreamProcessedCount)>? progress,
                Boolean leaveOpen)
            {
                Span<Byte> fixedHeader = stackalloc Byte[4];
                if (baseStream.ReadBytes(fixedHeader) != fixedHeader.Length)
                    throw new EndOfStreamException();
                var lengthOfContentProperties = fixedHeader.Slice(2, 2).ToUInt16LE();
                if (lengthOfContentProperties != LzmaDecoder.CONTENT_PROPERTY_SIZE)
                    throw new DataErrorException($"The format of the LZMA header is incorrect.");
                var contentProperties = new Byte[LzmaDecoder.CONTENT_PROPERTY_SIZE];
                if (baseStream.ReadBytes(contentProperties) != contentProperties.Length)
                    throw new EndOfStreamException();
                return new Decoder(
                    baseStream,
                    unpackedStreamSize,
                    progress,
                    leaveOpen,
                    stream => LzmaDecoder.CreateDecoderStream(stream, unpackedStreamSize, contentProperties));
            }
        }

        CompressionMethodId ICompressionCoder.CompressionMethodId => LzmaCoderPlugin.COMPRESSION_METHOD_ID;

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
            if (option is not ZipLzmaCompressionCoderOption)
                throw new ArgumentException($"Illegal {nameof(option)} data", nameof(option));

            return Decoder.Create(baseStream, unpackedStreamSize, progress, leaveOpen);
        }
    }
}
