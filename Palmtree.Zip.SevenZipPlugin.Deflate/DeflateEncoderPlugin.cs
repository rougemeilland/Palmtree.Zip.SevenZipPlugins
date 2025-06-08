using System;
using SevenZip.Compression;
using SevenZip.Compression.Deflate;

namespace Palmtree.IO.Compression.Stream.Plugin.SevenZip
{
    internal sealed class DeflateEncoderPlugin
        : ICompressionCoder, ICompressionEncoder
    {
        CompressionMethodId ICompressionCoder.CompressionMethodId => DeflateCoderPlugin.COMPRESSION_METHOD_ID;

        void IEncoder.Encode(
            ISequentialInputByteStream sourceStream,
            ISequentialOutputByteStream destinationStream,
            ICoderOption option,
            IProgress<(UInt64 inUncompressedStreamProcessedCount, UInt64 outCompressedStreamProcessedCount)>? progress)
        {
            if (sourceStream is null)
                throw new ArgumentNullException(nameof(sourceStream));
            if (destinationStream is null)
                throw new ArgumentNullException(nameof(destinationStream));
            if (option is null)
                throw new ArgumentNullException(nameof(option));
            if (option is not ZipDeflateCompressionCoderOption encoderOption)
                throw new ArgumentException($"Illegal {nameof(option)} data", nameof(option));

            using var encoder =
                DeflateEncoder.CreateEncoder(
                    new DeflateEncoderProperties
                    {
                        Level =
                            encoderOption.Level switch
                            {
                                ZipCompressionLevel.SuperFast => CompressionLevel.Fastest,
                                ZipCompressionLevel.Fast => CompressionLevel.Fast,
                                ZipCompressionLevel.Normal => CompressionLevel.Normal,
                                _ => CompressionLevel.Ultra,
                            },
                    });
            encoder.Code(
                sourceStream,
                destinationStream,
                null,
                null,
                progress);
        }
    }
}
