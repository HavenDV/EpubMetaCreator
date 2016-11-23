using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VersFx.Formats.Text.Epub;

namespace EpubMetaCreator
{
    class Program
    {
        public static string MetaHeader {
            get {
                return "\"Number of words\",\"File size in MB\",\"File Name\",\"Number of Images\"";
            }
        }
        public static string ImgsHeader {
            get {
                return "\"Image Width\",\"Image Height\",\"Pixel Depth\"";
            }
        }

        public static StreamWriter CreateMetaCSVFile(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var writer = File.CreateText(path);
            writer.WriteLine(MetaHeader);
            return writer;
        }

        public static StreamWriter CreateImgsCSVFile(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var writer = File.CreateText(path);
            writer.WriteLine(ImgsHeader);
            return writer;
        }

        static long WordCount(string text)
        {
            char[] delimiters = new char[] { ' ', '\r', '\n' };
            return text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }

        static void ReadEpub(string path)
        {
            var directory = Path.GetDirectoryName(path);
            var filename = Path.GetFileNameWithoutExtension(path);
            var metaCsv = CreateMetaCSVFile(Path.Combine(directory, filename + "_meta.csv"));
            var imgsCsv = CreateImgsCSVFile(Path.Combine(directory, filename + "_imgs.csv"));
            var info = new FileInfo(path);
            var size = info.Length;

            var book = EpubReader.ReadBook(path);
            var content = book.Content;
            // All images in the book (file name is the key)
            var images = content.Images;
            var imagesCount = images.Count;

            foreach (var imageContent in images.Values)
            {
                // Creating Image class instance from the content
                using (var stream = new MemoryStream(imageContent.Content))
                {
                    var image = Image.FromStream(stream);
                    imgsCsv.WriteLine($"{image.Width}, {image.Height}, {Image.GetPixelFormatSize(image.PixelFormat)}");
                }
            }

            // All XHTML files in the book (file name is the key)
            Dictionary<string, EpubTextContentFile> htmlFiles = content.Html;
            long wordCount = 0;

            // Entire HTML content of the book
            foreach (var htmlFile in htmlFiles.Values)
            {
                var html = htmlFile.Content;
                wordCount += WordCount(StripHTML(html));
            }

            metaCsv.WriteLine($"{wordCount}, {size}, {filename+".epub"}, {imagesCount}");
            metaCsv.Close();
            imgsCsv.Close();
        }

        static void Main(string[] args)
        {
            foreach (var file in Directory.GetFiles(".", "*.epub"))
            {
                ReadEpub(file);
            }
        }
    }
}
