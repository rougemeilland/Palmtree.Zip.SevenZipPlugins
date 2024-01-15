using System;
using Palmtree;
using Palmtree.Debug.IO;
using Palmtree.IO;
using Palmtree.IO.Compression.Archive.Zip;
using Palmtree.IO.Compression.Stream.Plugin.SevenZip;

namespace Test.CreateArchives
{
    internal class Program
    {
        private const string CLEAR_LINE = "\u001b[0K";
        private const string CARRIGE_RETURN = "\r";

        static Program()
        {
            Bzip2CoderPlugin.EnablePlugin();
            DeflateCoderPlugin.EnablePlugin();
            Deflate64CoderPlugin.EnablePlugin();
            LzmaCoderPlugin.EnablePlugin();
        }

        static void Main(string[] args)
        {
            const ulong MAX_CONTENT_LENGTH = 1024 * 1024;
            var baseDirectory = new DirectoryPath(args[0]);
            Test1(baseDirectory, "test_stored", MAX_CONTENT_LENGTH, ZipEntryCompressionMethodId.Stored);
            Test1(baseDirectory, "test_deflate", MAX_CONTENT_LENGTH, ZipEntryCompressionMethodId.Deflate);
            Test1(baseDirectory, "test_deflate64", MAX_CONTENT_LENGTH, ZipEntryCompressionMethodId.Deflate64);
            Test1(baseDirectory, "test_lzma", MAX_CONTENT_LENGTH, ZipEntryCompressionMethodId.LZMA);
            Console.WriteLine("終了しました。");
            Console.Beep();
            _ = Console.ReadLine();
        }

        private static void Test1(DirectoryPath baseDirectory, string fileName, ulong contentLength, ZipEntryCompressionMethodId compressionMethodId)
        {
            var zipArchive = baseDirectory.GetFile($"{fileName}.zip");
            using (var zipWriter = zipArchive.CreateAsZipFile())
            {
                {
                    var entry = zipWriter.CreateEntry("ディレクトリ/", "これはディレクトリです。");
                    entry.IsDirectory = true;
                    entry.CreationTimeUtc = DateTime.Now;
                    entry.LastAccessTimeUtc = DateTime.Now;
                    entry.LastWriteTimeUtc = DateTime.Now;
                }

                WriteZipEntry(zipArchive.Name, zipWriter, compressionMethodId, contentLength, "ディレクトリ/ファイル1.bin", "これはファイルその1です。");
                WriteZipEntry(zipArchive.Name, zipWriter, compressionMethodId, contentLength, "ディレクトリ/ファイル2.bin", "これはファイルその2です。");
                WriteZipEntry(zipArchive.Name, zipWriter, compressionMethodId, contentLength, "ディレクトリ/ファイル3.bin", "これはファイルその3です。");
                Console.WriteLine($"{CLEAR_LINE}書き込み終了: file=\"{zipArchive.Name}\"");
            }

            Console.Write($"{CLEAR_LINE}検査中... file=\"{zipArchive.FullName}\"{CARRIGE_RETURN}");

            using (var zipReader = zipArchive.OpenAsZipFile())
            {
                var exception = (Exception?)null;
                foreach (var entry in zipReader.EnumerateEntries())
                {
                    if (entry.IsFile)
                    {
                        var progressValue =
                            new ProgressValueHolder<(ulong inCompressedStreamProcessedCount, ulong outUncompressedStreamProcessedCount)>(
                                value => Console.Write($"{CLEAR_LINE}検査中... (in: {value.inCompressedStreamProcessedCount:N0} bytes, out: {value.outUncompressedStreamProcessedCount:N0} bytes), file=\"{zipArchive.Name}\".entry=\"{entry.FullName}\"{CARRIGE_RETURN}"),
                                (0, 0),
                                TimeSpan.FromMilliseconds(100));
                        progressValue.Report();
                        using (var inStream = entry.OpenContentStream(new SimpleProgress<(ulong inCompressedStreamProcessedCount, ulong outUncompressedStreamProcessedCount)>(value => progressValue.Value = value)))
                        using (var outStream = OutputTestDataStream.Create(ex => exception = ex))
                        {
                            inStream.CopyTo(outStream);
                        }

                        progressValue.Report();
                        if (exception is not null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            try
                            {
                                Console.WriteLine($"{CLEAR_LINE}{exception.Message}, file=\"{zipArchive.Name}\", entry=\"{entry.FullName}\"");
                            }
                            finally
                            {
                                Console.ResetColor();
                            }
                        }
                    }
                }
            }

            Console.WriteLine($"{CLEAR_LINE}検査終了: file=\"{zipArchive.Name}\"");
        }

        private static void WriteZipEntry(string zipFileName, ZipArchiveFileWriter zipWriter, ZipEntryCompressionMethodId compressionMethodId, ulong contentLength, string entryName, string entryComment)
        {
            Console.Write($"{CLEAR_LINE}書き込み中... file=\"{zipFileName}\"{CARRIGE_RETURN}");
            var entry = zipWriter.CreateEntry(entryName, entryComment);
            Console.Write($"{CLEAR_LINE}書き込み中... file=\"{zipFileName}\", entry=\"{entry.FullName}\"{CARRIGE_RETURN}");
            entry.IsFile = true;
            entry.CreationTimeUtc = DateTime.Now;
            entry.LastAccessTimeUtc = DateTime.Now;
            entry.LastWriteTimeUtc = DateTime.Now;
            entry.CompressionMethodId = compressionMethodId;
            entry.CompressionLevel = ZipEntryCompressionLevel.Maximum;
            var progressValue =
                new ProgressValueHolder<(ulong inCompressedStreamProcessedCount, ulong outUncompressedStreamProcessedCount)>(
                    value => Console.Write($"{CLEAR_LINE}書き込み中... (in: {value.inCompressedStreamProcessedCount:N0} bytes, out: {value.outUncompressedStreamProcessedCount:N0} bytes), file=\"{zipFileName}\".entry=\"{entry.FullName}\"{CARRIGE_RETURN}"),
                    (0, 0),
                    TimeSpan.FromMilliseconds(100));
            progressValue.Report();
            using (var inStream = InputTestDataStream.Create(contentLength, b => (byte)(b & 0x3f)))
            using (var outStream1 = entry.CreateContentStream(new SimpleProgress<(ulong inCompressedStreamProcessedCount, ulong outUncompressedStreamProcessedCount)>(value => progressValue.Value = value)))
            {
                inStream.CopyTo(outStream1);
            }

            progressValue.Report();
        }
    }
}
