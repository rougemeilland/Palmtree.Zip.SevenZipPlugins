using System;
using System.Linq;
using System.Text;
using Palmtree;
using Palmtree.IO;
using Palmtree.IO.Compression.Archive.Zip;
using Palmtree.IO.Compression.Stream.Plugin;
using Palmtree.IO.Compression.Stream.Plugin.SevenZip;

namespace Test.Validation
{
    internal sealed class Program
    {
        static Program()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            StoredCoderPlugin.EnablePlugin();
            Bzip2CoderPlugin.EnablePlugin();
            DeflateCoderPlugin.EnablePlugin();
            Deflate64CoderPlugin.EnablePlugin();
            LzmaCoderPlugin.EnablePlugin();
        }

        private static void Main(string[] args)
        {
            var fileList =
                args.EnumerateFilesFromArgument(true)
                .Where(file => file.Extension.IsAnyOf(".zip", ".001", ".exe", ".epub", StringComparison.OrdinalIgnoreCase))
                .ToList();
            var totalSize = fileList.Aggregate(0UL, (length, file) => checked(length + file.Length));
            var completed = 0UL;
            foreach (var file in fileList)
            {
                var progressCounter = new ProgressCounter<double>(value => Console.Write($"  {(completed + value * file.Length) * 100.0 / totalSize:F2}%\r"), 0, TimeSpan.FromMilliseconds(100));
                progressCounter.Report();
                try
                {
                    var result = file.ValidateAsZipFile(ValidationStringency.Strict | ValidationStringency.AllowNullPayloadAfterEOCDR, new SimpleProgress<double>(value => progressCounter.Value = value));
                    if (result.ResultId != ZipArchiveValidationResultId.Ok)
                        Console.ForegroundColor = ConsoleColor.Red;
                    else
                        progressCounter.Report();
                    checked
                    {
                        completed += file.Length;
                    }

                    Console.WriteLine($"\"{(double)completed * 100 / totalSize:F2}% {file.FullName}\": {result.ResultId}, \"{result.Message}\"");
                }
                finally
                {
                    Console.ResetColor();
                }
            }

            Console.WriteLine("Completed.");
            Console.Beep();
            _ = Console.ReadLine();
        }
    }
}
