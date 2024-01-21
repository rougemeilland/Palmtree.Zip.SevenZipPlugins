namespace Palmtree.IO.Compression.Stream.Plugin.SevenZip
{
    public class Deflate64CoderPlugin
        : ICompressionCoderPlugin
    {
        internal const CompressionMethodId COMPRESSION_METHOD_ID = CompressionMethodId.Deflate64;

        private Deflate64CoderPlugin()
        {
        }

        public static void EnablePlugin()
        {
            CompressionCoderPlugin.Register(new Deflate64DecoderPlugin());
            CompressionCoderPlugin.Register(new Deflate64EncoderPlugin());
        }
    }
}
