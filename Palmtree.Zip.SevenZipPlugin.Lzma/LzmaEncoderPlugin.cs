using System;
using SevenZip.Compression;
using SevenZip.Compression.Lzma;

namespace Palmtree.IO.Compression.Stream.Plugin.SevenZip
{
    internal sealed class LzmaEncoderPlugin
        : ICompressionCoder, ICompressionEncoder
    {
        CompressionMethodId ICompressionCoder.CompressionMethodId => LzmaCoderPlugin.COMPRESSION_METHOD_ID;

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
            if (option is not ZipLzmaCompressionCoderOption encoderOption)
                throw new ArgumentException($"Illegal {nameof(option)} data", nameof(option));

            using var encoder
                = LzmaEncoder.CreateEncoder(
                    new LzmaEncoderProperties
                    {
                        Level =
                            encoderOption.Level switch
                            {
                                ZipCompressionLevel.SuperFast => CompressionLevel.Fastest,
                                ZipCompressionLevel.Fast => CompressionLevel.Fast,
                                ZipCompressionLevel.Normal => CompressionLevel.Normal,
                                _ => CompressionLevel.Ultra,
                            },
                        EndMarker = encoderOption.UseEndOfStreamMarker,
                    });
            destinationStream.WriteUInt16LE(LzmaCoderPlugin.LzmaSdkVetrsion);
            destinationStream.WriteUInt16LE(LzmaEncoder.CONTENT_PROPERTY_SIZE);
            encoder.WriteCoderProperties(destinationStream);
            encoder.Code(
                sourceStream,
                destinationStream,
                null,
                null,
                progress);
        }
    }
}
