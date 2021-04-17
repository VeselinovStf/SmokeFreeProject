using System;

namespace SmokeFree.Utilities.Zip
{
    /// <summary>
    /// File Zipping Utility
    /// </summary>
    public class FileZipUtility
    {
        /// <summary>
        /// Application Logger
        /// </summary>
        private static NLog.ILogger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Zip directory files
        /// </summary>
        /// <param name="directoryToZip">Directory to zip</param>
        /// <param name="destinationZipFullPath">Destination</param>
        /// <returns></returns>
        public static bool QuickZip(string directoryToZip, string destinationZipFullPath)
        {
            try
            {
                // Delete existing zip file if exists
                if (System.IO.File.Exists(destinationZipFullPath))
                    System.IO.File.Delete(destinationZipFullPath);
                if (!System.IO.Directory.Exists(directoryToZip))
                    return false;
                else
                {
                    System.IO.Compression.ZipFile.CreateFromDirectory(directoryToZip, destinationZipFullPath, System.IO.Compression.CompressionLevel.Optimal, true);

                    return System.IO.File.Exists(destinationZipFullPath);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                return false;
            }
        }
    }
}
