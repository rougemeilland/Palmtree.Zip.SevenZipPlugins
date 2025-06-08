using System;
using SevenZip.Compression;
using SevenZip.Compression.Deflate64;

namespace Palmtree.IO.Compression.Stream.Plugin.SevenZip
{
    internal sealed class Deflate64EncoderPlugin
        : ICompressionCoder, ICompressionEncoder
    {
        CompressionMethodId ICompressionCoder.CompressionMethodId => Deflate64CoderPlugin.COMPRESSION_METHOD_ID;

        void IEncoder.Encode(
            ISequentialInputByteStream sourceStream,
            ISequentialOutputByteStream destinationStream,
            ICoderOption option,
            IProgress<(UInt64 inUncompressedStreamProcessedCount, UInt64 outCompressedStreamProcessedCount)>? progress)
        {
            ArgumentNullException.ThrowIfNull(sourceStream);
            ArgumentNullException.ThrowIfNull(destinationStream);
            ArgumentNullException.ThrowIfNull(option);
            if (option is not ZipDeflate64CompressionCoderOption encoderOption)
                throw new ArgumentException($"Illegal {nameof(option)} data", nameof(option));

            using var encoder =
                Deflate64Encoder.CreateEncoder(
                    new Deflate64EncoderProperties
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
