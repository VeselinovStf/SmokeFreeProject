using SmokeFree.Models.Utilities.Zip;
using System.Reflection;

namespace SmokeFree.Abstraction.Utility.Logging
{
    /// <summary>
    /// Device Specific Local Logging Utility Abstraction
    /// </summary>
    public interface ILocalLogUtility
    {
        /// <summary>
        /// Initialize Logger
        /// </summary>
        /// <param name="assembly">Project assemblu</param>
        /// <param name="assemblyName">Assembly name</param>
        void Initialize(Assembly assembly, string assemblyName);

        /// <summary>
        /// Create Log Zip File
        /// </summary>
        /// <returns>Response Model</returns>
        CreateLogZipFileResponse CreateLogZipFile();

        /// <summary>
        /// Creates Db ZIp
        /// </summary>
        /// <returns>Response Model</returns>
        CreateDbZipFileResponse CreateDbZipFile(string dbPath, string[] content);
    }
}
