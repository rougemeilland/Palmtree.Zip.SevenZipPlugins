using System;
using SevenZip.Compression;

namespace Palmtree.IO.Compression.Stream.Plugin.SevenZip
{
    public class LzmaCoderPlugin
        : ICompressionCoderPlugin
    {
        internal const CompressionMethodId COMPRESSION_METHOD_ID = CompressionMethodId.LZMA;

        internal static readonly UInt16 LzmaSdkVetrsion;

        static LzmaCoderPlugin()
        {
            var majorVersion = (Byte)(SevenZipModule.Versio >> 16);
            var minorVersion = (Byte)SevenZipModule.Versio;
            LzmaSdkVetrsion = checked((UInt16)((majorVersion << 8) | minorVersion));
        }

        public static void EnablePlugin()
        {
            CompressionCoderPlugin.Register(new LzmaDecoderPlugin());
            CompressionCoderPlugin.Register(new LzmaEncoderPlugin());
        }
    }
}
