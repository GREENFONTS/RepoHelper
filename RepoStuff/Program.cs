using System.Diagnostics;
using System.Text.Json;

namespace RepoStuff
{

    public class RepoMain {
        public static async Task Main(string[] args)
        {
            //7180a8a4485494323ebab11c36c62add00aa8ba0 secret
            //7884a1308e3c1e372d71 client id

            string clientId = "7884a1308e3c1e372d71";
            string scope = "repo";

            //get Device code and Verification_url
            var CallResponse =  await GitFlow.GetDeviceCode(clientId, scope);

            //copy user_code to clipBoard
            var user_code = Convert.ToString(CallResponse["user_code"])!;
            GitFlow.SaveToClipboard(user_code);

            //open a browser and mobe to the verification url
            var verification_url = Convert.ToString(CallResponse["verification_uri"])!;
            var interval = Convert.ToString(CallResponse["interval"])!;
            var deviceCode = Convert.ToString(CallResponse["device_code"])!;

            await GitFlow.GetAccessToken(verification_url, clientId, deviceCode, interval);
           

            Console.WriteLine(deviceCode);
            Console.WriteLine(verification_url);

            Console.ReadLine();

        }
    }

    public static class GitFlow {

        private static readonly HttpClient client = new HttpClient();

        public static async Task<IDictionary<string, object>> GetDeviceCode(string clientId, string scope)
        {
            
            GitFlow.client.DefaultRequestHeaders.Add("Accept", "application/json");
            var formBody = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("scope", scope)
            });
            var url = new Uri("https://github.com/login/device/code");
            var res = await GitFlow.client.PostAsync(url, formBody);
            if (res.IsSuccessStatusCode)
            {
                var result = await res.Content.ReadAsStringAsync();
                var content = JsonSerializer.Deserialize<IDictionary<string, object>>(result)!;

                return content;
            }
            else
            {
                Console.WriteLine(res.StatusCode);
                return null;
            }
        }

        public static void SaveToClipboard(string text)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "PowerShell",
                ArgumentList = { $"Set-Clipboard -Value \"{text}\"" },
                RedirectStandardInput = true
            });
        }


        public static void StartUpBrowser(string url)
        {
            Process.Start(new ProcessStartInfo { 
                FileName = "PowerShell",
                ArgumentList = { "Start-Process", "msedge",  url}
            });

        }

        public async static Task<IDictionary<string,object>> RequestAccessToken(KeyValuePair<string, string>[] Params)
        {
            var formBody = new FormUrlEncodedContent(Params);
            GitFlow.client.DefaultRequestHeaders.Add("Accept", "application/json");
            //Console.WriteLine(Convert.ToString(Params["client_id"]);

            var url = new Uri("https://github.com/login/oauth/access_token");
            var res = await GitFlow.client.PostAsync(url, formBody);

            if (res.IsSuccessStatusCode)
            {
                var result = await res.Content.ReadAsStringAsync();
                var content = JsonSerializer.Deserialize<IDictionary<string, object>>(result)!;

                return content;
            }
            else
            {
                Console.WriteLine(res.StatusCode);
                return null;
            }
        }
        public async static Task GetAccessToken(string url, string clientId, string deviceCode, string interval)
        {
            StartUpBrowser(url);


            var Params = new[] {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("device_code", deviceCode),
                new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth: grant - type:device_code")
            };

            while (true)
            {
                var res = await RequestAccessToken(Params);
                Console.WriteLine(res);

                if (res.ContainsKey("error"))
                {
                    Console.WriteLine("enetered");
                    var delay = Convert.ToDouble(interval);
                    Console.WriteLine(delay);
                    await Task.Delay(TimeSpan.FromSeconds(delay));
                }
                else
                {
                    Console.WriteLine("success");
                    var token = Convert.ToString(res["access_token"]);
                    Console.WriteLine(token);
                }
            }
        }

    }

   


    
}