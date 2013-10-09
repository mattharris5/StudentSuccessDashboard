using System.IO;

namespace SSD
{
    public static class StringExtensions
    {
        public static string GetSafeFileName(this string fileName)
        {
            return GetSafeFileName(fileName, '_');
        }

        public static string GetSafeFileName(this string fileName, char replaceChar)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, replaceChar);
            }
            return fileName;
        }
    }
}
