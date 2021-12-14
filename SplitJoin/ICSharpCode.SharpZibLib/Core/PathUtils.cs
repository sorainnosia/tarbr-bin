namespace ICSharpCode.SharpZipLib.Core
{
    /// <summary>
    /// PathUtils provides simple utilities for handling paths.
    /// </summary>
    public static class PathUtils
    {

        public static char[] FilterChar(IEnumerable<char> str, char[] invalidChars, bool cleanRootSep)
        {
            if (invalidChars == null) invalidChars = new char[0];
            char[] result = new char[0];
            IEnumerator<char> e = str.GetEnumerator();

            int i = 0;
            while (e.MoveNext())
            {
                if (invalidChars.Contains(e.Current) || (i == 2 && cleanRootSep))
                    result = ArrayUtility.ArrayUtil.AddArray(result, '_');
                else
                    result = ArrayUtility.ArrayUtil.AddArray(result, e.Current);
                i++;
            }
            return result;
        }

        /// <summary>
        /// Remove any path root present in the path
        /// </summary>
        /// <param name="path">A <see cref="string"/> containing path information.</param>
        /// <returns>The path with the root removed if it was present; path otherwise.</returns>
        public static string DropPathRoot(string path)
        {
            var invalidChars = Path.GetInvalidPathChars();
            // If the first character after the root is a ':', .NET < 4.6.2 throws
            var cleanRootSep = path.Length >= 3 && path[1] == ':' && path[2] == ':';

            // Replace any invalid path characters with '_' to prevent Path.GetPathRoot from throwing.
            // Only pass the first 258 (should be 260, but that still throws for some reason) characters
            // as .NET < 4.6.2 throws on longer paths
            var cleanPath = new string(FilterChar(path.Take(258), invalidChars, cleanRootSep));
            //.Select( (c, i) => invalidChars.Contains(c) || (i == 2 && cleanRootSep) ? '_' : c).ToArray());

            var stripLength = Path.GetPathRoot(cleanPath).Length;
            while (path.Length > stripLength && (path[stripLength] == '/' || path[stripLength] == '\\')) stripLength++;
            return path.Substring(stripLength);
        }

        /// <summary>
        /// Returns a random file name in the users temporary directory, or in directory of <paramref name="original"/> if specified
        /// </summary>
        /// <param name="original">If specified, used as the base file name for the temporary file</param>
        /// <returns>Returns a temporary file name</returns>
        public static string GetTempFileName(string original = null)
        {
            string fileName;
            var tempPath = Path.GetTempPath();

            do
            {
                fileName = original == null
                    ? Path.Combine(tempPath, Path.GetRandomFileName())
                    : $"{original}.{Path.GetRandomFileName()}";
            } while (File.Exists(fileName));

            return fileName;
        }
    }
}
