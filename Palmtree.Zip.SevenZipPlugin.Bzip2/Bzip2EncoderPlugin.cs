using System;
using SevenZip.Compression;
using SevenZip.Compression.Bzip2;

namespace Palmtree.IO.Compression.Stream.Plugin.SevenZip
{
    internal sealed class Bzip2EncoderPlugin
        : ICompressionCoder, ICompressionEncoder
    {
        CompressionMethodId ICompressionCoder.CompressionMethodId => Bzip2CoderPlugin.COMPRESSION_METHOD_ID;

        void IEncoder.Encode(
            ISequentialInputByteStream sourceStream,
            ISequentialOutputByteStream destinationStream,
            ICoderOption option,
            IProgress<(UInt64 inUncompressedStreamProcessedCount, UInt64 outCompressedStreamProcessedCount)>? progress)
        {
            ArgumentNullException.ThrowIfNull(sourceStream);
            ArgumentNullException.ThrowIfNull(destinationStream);
            ArgumentNullException.ThrowIfNull(option);
            if (option is not ZipBzip2CompressionCoderOption encoderOption)
                throw new ArgumentException($"Illegal {nameof(option)} data", nameof(option));

            using var encoder =
                Bzip2Encoder.CreateEncoder(
                    new Bzip2EncoderProperties
                    {
                        Level =
                            encoderOption.Level switch
                            {
                                ZipCompressionLevel.SuperFast => CompressionLevel.Fastest,
                                ZipCompressionLevel.Fast => CompressionLevel.Fast,
                                ZipCompressionLevel.Normal => CompressionLevel.Normal,
                                _ => CompressionLevel.Ultra,
                            }
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
