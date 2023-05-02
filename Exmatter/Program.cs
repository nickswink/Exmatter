using System;
using System.IO;
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
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: Exmatter.exe dropbox <file-path> <access-token>");
                Console.WriteLine("Description: uploads to dropbox using access-token");
                Environment.Exit(1);
            }

            string uploadFilePath = args[1];
            string accessToken = args[2];

            var uploader = new DropboxUploader(accessToken);

            // Call the UploadFileAsync method to upload a file to Dropbox
            await uploader.UploadFileAsync(uploadFilePath);

        }
        static async Task HandleSlackCommand(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: Exmatter.exe slack <file-path> <API key> <slack channel name>");
                Console.WriteLine("Description: uploads to slack channel");
                Environment.Exit(1);
            }

            string uploadFilePath = args[1];
            string apiKey = args[2];
            string slackChannel = args[3];

            // do stuff

        }



        static async Task Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: Exmatter.exe dropbox|slack <file-path> <access-token>");
                Console.WriteLine("Description: Take a guess.");
                Environment.Exit(1);
            }

            var subCommand = args[0].ToLower();

            switch (subCommand)
            {
                case "dropbox":
                    await HandleDropboxCommand(args);
                    break;

                case "slack":
                    await HandleSlackCommand(args);
                    break;

                default:
                    Console.WriteLine($"[!] Unknown sub-command: {subCommand}");
                    Console.WriteLine("Available sub-commands: dropbox, slack");
                    Environment.Exit(1);
                    break;
            }

           
        }
    }
}
