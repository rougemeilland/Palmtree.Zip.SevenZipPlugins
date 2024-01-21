namespace Palmtree.IO.Compression.Stream.Plugin.SevenZip
{
    public class Bzip2CoderPlugin
        : ICompressionCoderPlugin
    {
        internal const CompressionMethodId COMPRESSION_METHOD_ID = CompressionMethodId.BZIP2;

        private Bzip2CoderPlugin()
        {
        }

        public static void EnablePlugin()
        {
            CompressionCoderPlugin.Register(new Bzip2DecoderPlugin());
            CompressionCoderPlugin.Register(new Bzip2EncoderPlugin());
        }
    }
}
