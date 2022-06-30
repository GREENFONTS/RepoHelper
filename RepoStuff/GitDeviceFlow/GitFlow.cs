using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RepoStuff.GitDeviceFlow
{
    public static class GitFlow
    {   private static HttpClient _client = new HttpClient();
       
        //Function to get the device access token
        public async static Task<string> GetAccessToken(string clientId, string scope)
        {
            Console.WriteLine("Welcome, please follow through the one time authentication Process");

            //get Device code and other parameters
            var CallResponse = await GetDeviceCode(clientId, scope);

            //parameter declarations
            var user_code = Convert.ToString(CallResponse["user_code"])!;
            var verification_url = Convert.ToString(CallResponse["verification_uri"])!;
            var interval = Convert.ToString(CallResponse["interval"])!;
            var deviceCode = Convert.ToString(CallResponse["device_code"])!;

            //copy user_code to clipBoard
            SaveToClipboard(user_code);

            //start up browser
            StartUpBrowser(verification_url);


            var Params = new[] {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("device_code", deviceCode),
                new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:device_code")
            };

            while (true)
            {
                var res = await RequestAccessToken(Params);

                if (res.ContainsKey("error"))
                {
                    var delay = Convert.ToDouble(interval);
                    await Task.Delay(TimeSpan.FromSeconds(delay));
                }
                else
                {
                    var token = Convert.ToString(res["access_token"])!;
                    return token;
                }
            }
        }


        //Function to get the device code and verification uri
        public static async Task<IDictionary<string, object>> GetDeviceCode(string clientId, string scope)
        {

            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            var formBody = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("scope", scope)
            });
            var url = new Uri("https://github.com/login/device/code");
            var res = await _client.PostAsync(url, formBody);
            if (res.IsSuccessStatusCode)
            {
                var result = await res.Content.ReadAsStringAsync();
                var content = JsonSerializer.Deserialize<IDictionary<string, object>>(result)!;

                return content;
            }
            else
            {
                Console.WriteLine(res.StatusCode);
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
        }

        //Function to save the device Code to the clipboard
        public static void SaveToClipboard(string text)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "PowerShell",
                ArgumentList = { $"Set-Clipboard -Value \"{text}\"" },
                RedirectStandardInput = true
            });

            Console.WriteLine("User-Code copied to clipboard");
        }

        //Function to start up the browser and input the device code
        public static void StartUpBrowser(string url)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "PowerShell",
                ArgumentList = { "Start-Process", "msedge", url }
            });

            Console.WriteLine("Please Press Crtl-V to paste the code in the Open github authentication browser");

        }

        //Function to request access token
        public async static Task<IDictionary<string, object>> RequestAccessToken(KeyValuePair<string, string>[] Params)
        {
            var formBody = new FormUrlEncodedContent(Params);
            _client.DefaultRequestHeaders.Add("Accept", "application/json");

            var url = new Uri("https://github.com/login/oauth/access_token");
            var res = await _client.PostAsync(url, formBody);

            if (res.IsSuccessStatusCode)
            {
                var result = await res.Content.ReadAsStringAsync();
                var content = JsonSerializer.Deserialize<IDictionary<string, object>>(result)!;

                return content;
            }
            else
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
        }

    }
}
