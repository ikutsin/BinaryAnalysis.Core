using System;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace BinaryAnalysis.UI.Commons
{
    public static class ZipFunctions
    {
        public static class Functions
        {
            public static bool IfNewer(ZipEntry zipEntry, String fullZipToPath)
            {
                return File.Exists(fullZipToPath)
                       && File.GetCreationTime(fullZipToPath) > zipEntry.DateTime;
            }
            public static bool IfDifferentSize(ZipEntry zipEntry, String fullZipToPath)
            {
                FileInfo f = new FileInfo(fullZipToPath);
	            long filesize = f.Length;
                return File.Exists(fullZipToPath) && filesize != zipEntry.Size;
            }
            public static bool IfNotSame(ZipEntry zipEntry, String fullZipToPath)
            {
                return IfNewer(zipEntry, fullZipToPath)
                       || IfDifferentSize(zipEntry, fullZipToPath);
            }
        }

        public static void ExtractZipFile(string archiveFilenameIn, string outFolder, 
            Func<ZipEntry, String, bool> entryCheckFunc,
            string password = null)
        {
            ZipFile zf = null;
            try
            {
                FileStream fs = File.OpenRead(archiveFilenameIn);
                zf = new ZipFile(fs);
                if (!String.IsNullOrEmpty(password))
                {
                    zf.Password = password;		// AES encrypted entries are handled automatically
                }
                foreach (ZipEntry zipEntry in zf)
                {
                    if (!zipEntry.IsFile)
                    {
                        continue;			// Ignore directories
                    }
                    String entryFileName = zipEntry.Name;

                    // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                    // Optionally match entrynames against a selection list here to skip as desired.
                    // The unpacked length is available in the zipEntry.Size property.

                    // Manipulate the output filename here as desired.
                    String fullZipToPath = Path.Combine(outFolder, entryFileName);
                    string directoryName = Path.GetDirectoryName(fullZipToPath);

                    if (entryCheckFunc(zipEntry, fullZipToPath)) continue;

                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);

                    byte[] buffer = new byte[4096];		// 4K is optimum
                    Stream zipStream = zf.GetInputStream(zipEntry);


                    // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                    // of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    using (FileStream streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ExtractZipFile failed", ex);
            }
            finally
            {
                if (zf != null)
                {
                    zf.IsStreamOwner = true; // Makes close also shut the underlying stream
                    zf.Close(); // Ensure we release resources
                }
            }
        }
    }
}
