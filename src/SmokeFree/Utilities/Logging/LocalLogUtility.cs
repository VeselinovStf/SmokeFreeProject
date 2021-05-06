using NLog;
using NLog.Config;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Models.Utilities.Zip;
using SmokeFree.Utilities.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace SmokeFree.Utilities.Logging
{
    public class LocalLogUtility : ILocalLogUtility
    {
        /// <summary>
        /// Initialize Local Logging Utility
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <param name="assemblyName">Assembly Name</param>
        public void Initialize(Assembly assembly, string assemblyName)
        {
            string resourcePrefix = string.Empty;

            if (Device.RuntimePlatform == Device.iOS)
            {
                resourcePrefix = "SmokeFree.iOS";
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                resourcePrefix = "SmokeFree.Droid";
            }
            else if (Device.RuntimePlatform == Device.UWP)
            {
                resourcePrefix = "SmokeFree.Uwp";
            }
            else
            {
                throw new Exception("Could not initialize Logger: Unknonw Platform");
            }

            string location = $"{resourcePrefix}.NLog.config";

            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(location))
                {
                    if (stream == null)
                    {
                        throw new Exception($"The resource '{location}' was not loaded properly.");
                    }

                    LogManager.Configuration = new XmlLoggingConfiguration(System.Xml.XmlReader.Create(stream), null);
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"[FATAL] : Logger Exception : {ex}");
            }
        }

        /// <summary>
        /// Generate Log Zip File
        /// </summary>
        /// <returns>Response Model</returns>
        public CreateLogZipFileResponse CreateLogZipFile()
        {
            try
            {


                string zipFilename = string.Empty;

                // Check Global Logging Enabled
                if (NLog.LogManager.IsLoggingEnabled())
                {
                    string folder;

                    // Device Specific - Get Log Folder Path
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        folder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "..", "Library");
                    }
                    else if (Device.RuntimePlatform == Device.Android)
                    {
                        folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    }
                    else
                    {
                        throw new Exception("Could not show log: Platform undefined.");
                    }

                    // Delete old zipfiles (housekeeping)
                    try
                    {
                        foreach (string fileName in System.IO.Directory.GetFiles(folder, "*.zip"))
                        {
                            System.IO.File.Delete(fileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error deleting old zip files: {ex.Message}");

                        return new CreateLogZipFileResponse(false, ex.Message);
                    }

                    string logFolder = System.IO.Path.Combine(folder, "logs");

                    // If Logs Folder Exist
                    if (System.IO.Directory.Exists(logFolder))
                    {
                        // Zip Name
                        var zipName = $"{DateTime.Now.ToString("yyyy-MM-dd-HH_mm_ss")}.zip";
                        zipFilename = $"{folder}/{zipName}";

                        int filesCount = System.IO.Directory.GetFiles(logFolder, "*.csv").Length;

                        // If contains any logs
                        if (filesCount > 0)
                        {
                            // Zip csv-s
                            if (!FileZipUtility.QuickZip(logFolder, zipFilename))
                            {
                                return new CreateLogZipFileResponse(
                                    false,
                                    $"Can't zip log files! Log Folder: {logFolder}, Zip File Name: {zipFilename}");
                            }
                            else
                            {
                                var zipFile = System.IO.Directory.GetFiles(folder, zipName).FirstOrDefault();

                                return new CreateLogZipFileResponse(true, zipFile);
                            }
                        }
                        else
                        {
                            return new CreateLogZipFileResponse(false, $"Can't find .csv files to Zip! Log Folder: {logFolder}");
                        }
                    }
                    else
                    {
                        return new CreateLogZipFileResponse(false, $"Can't find log folder! Log Folder: {logFolder}");
                    }

                }

                return new CreateLogZipFileResponse(false, "Application Logging Is Not Enabled");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                return new CreateLogZipFileResponse(false, ex.Message);
            }

        }

        public CreateDbZipFileResponse CreateDbZipFile(string dbPath, string[] content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dbPath))
                {
                    return new CreateDbZipFileResponse(false, "Invalid DB Path");
                }

                string zipFilename = string.Empty;

                string folder;

                // Device Specific - Get Log Folder Path
                if (Device.RuntimePlatform == Device.iOS)
                {
                    folder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "..", "Library");
                }
                else if (Device.RuntimePlatform == Device.Android)
                {
                    folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                }
                else
                {
                    throw new Exception("Could not locate folder structure: Platform undefined.");
                }

                // Delete old zipfiles (housekeeping)
                try
                {
                    foreach (string fileName in System.IO.Directory.GetFiles(folder, "*.db.zip"))
                    {
                        System.IO.File.Delete(fileName);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error deleting old zip files: {ex.Message}");

                    return new CreateDbZipFileResponse(false, ex.Message);
                }

                string logFolder = System.IO.Path.Combine(folder, "logs");

                // If Logs Folder Exist
                if (System.IO.Directory.Exists(logFolder))
                {
                    // Zip Name
                    var zipName = $"{DateTime.Now.ToString("DB_yyyy-MM-dd-HH_mm_ss")}.db.zip";
                    zipFilename = $"{folder}/{zipName}";


                    int filesCount = System.IO.Directory.GetFiles(dbPath, "*.realm").Length;

                    // If contains any logs
                    if (filesCount > 0)
                    {
                        var coppyDbFolder = "CoppyDbFolder";
                        var coppyFolderPath = Path.Combine(folder, coppyDbFolder);

                        if (!Directory.Exists(coppyFolderPath))
                        {
                            Directory.CreateDirectory(Path.Combine(folder, coppyDbFolder));
                        }
                        else
                        {
                            foreach (string fileName in System.IO.Directory.GetFiles(coppyFolderPath, "*.txt"))
                            {
                                System.IO.File.Delete(fileName);
                            }
                        }

                        var txtFileName = "temp_db.txt";
                        var newTxtDbPath = Path.Combine(coppyFolderPath, txtFileName);
                        using (var fileStream = new StreamWriter(newTxtDbPath))
                        {
                            foreach (var c in content)
                            {
                                fileStream.WriteLine(c);
                            }

                        }

                        // Zip csv-s
                        if (!FileZipUtility.QuickZip(coppyFolderPath, zipFilename))
                        {
                            return new CreateDbZipFileResponse(
                                false,
                                $"Can't zip db files! Log Folder: {logFolder}, Zip File Name: {zipFilename}");
                        }
                        else
                        {
                            var zipFile = System.IO.Directory.GetFiles(folder, zipName).FirstOrDefault();

                            return new CreateDbZipFileResponse(true, zipFile);
                        }
                    }
                    else
                    {
                        return new CreateDbZipFileResponse(false, $"Can't find .realm files to Zip! DB Folder: {logFolder}");
                    }
                }
                else
                {
                    return new CreateDbZipFileResponse(false, $"Can't find db folder! Db Folder: {logFolder}");
                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                return new CreateDbZipFileResponse(false, ex.Message);
            }
        }
    }
}
