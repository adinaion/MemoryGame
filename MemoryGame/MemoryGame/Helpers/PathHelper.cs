using System;
using System.IO;

namespace MemoryGame.Helpers
{
    public static class PathHelper
    {
        public static string GetRelativePath(string baseDir, string absolutePath)
        {
            if (!baseDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                baseDir += Path.DirectorySeparatorChar;

            Uri baseUri = new Uri(baseDir);
            Uri absoluteUri = new Uri(absolutePath);
            string relativePath = Uri.UnescapeDataString(baseUri.MakeRelativeUri(absoluteUri).ToString());
            return relativePath.Replace('/', Path.DirectorySeparatorChar);
        }
    }
}