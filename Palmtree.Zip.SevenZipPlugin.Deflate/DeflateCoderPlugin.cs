namespace Palmtree.IO.Compression.Stream.Plugin.SevenZip
{
    public class DeflateCoderPlugin
        : ICompressionCoderPlugin
    {
        internal const CompressionMethodId COMPRESSION_METHOD_ID = CompressionMethodId.Deflate;

        private DeflateCoderPlugin()
        {
        }

        public static void EnablePlugin()
        {
            CompressionCoderPlugin.Register(new DeflateDecoderPlugin());
            CompressionCoderPlugin.Register(new DeflateEncoderPlugin());
        }
    }
}
