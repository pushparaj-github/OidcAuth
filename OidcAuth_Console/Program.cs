using System.Configuration;
using IdentityModel.OidcClient;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace OidcAuth;

internal class Program
{
    static void Main(string[] args)
    {
        var accessToken = GetUserDetails();
        Console.WriteLine("===================================");
        Console.WriteLine($"AccessToken: {accessToken}");
        Console.WriteLine("===================================");
    }

    public static string GetUserDetails()
    {
        var accessToken = MainAsync().GetAwaiter().GetResult();
        return accessToken;        
    }
    public static async Task<string> MainAsync()
    {
        var authority = ConfigurationSettings.AppSettings["Authority"];
        var clientid = ConfigurationSettings.AppSettings["ClientId"];
        var redirectUrl = ConfigurationSettings.AppSettings["RedirectUrl"];

        var accessToken = await SignIn(authority, clientid, redirectUrl);
        return accessToken.ToString();
        //Console.WriteLine("===================================");
        //Console.WriteLine($"AccessToken: {accessToken}");
        //Console.WriteLine("===================================");
    }
    private static async Task<string> SignIn(string authority, string clientId, string redirectUrl)
    {
        var http = new HttpListener();
        http.Prefixes.Add(redirectUrl);
        http.Start();

        var options = new OidcClientOptions
        {
            Authority = authority,
            ClientId = clientId,
            Scope = "openid profile api",
            RedirectUri = redirectUrl,
        };

        var client = new OidcClient(options);
        var state = await client.PrepareLoginAsync();

        Console.WriteLine($"Start URL: {state.StartUrl}");

        Process.Start(new ProcessStartInfo
        {
            FileName = state.StartUrl,
            UseShellExecute = true,
        });

        var context = await http.GetContextAsync();
        var response = context.Response;
        var responseString = $"<html><head><meta http-equiv='refres' content='10;url={authority}></head><body>Please returh to the app</body></html>";
        var buffer = Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        var responseOutput = response.OutputStream;
        await responseOutput.WriteAsync(buffer, 0, buffer.Length);
        responseOutput.Close();

        var result = await client.ProcessResponseAsync(context.Request.RawUrl, state);

        if (result.IsError)
        {
            Console.WriteLine("Error", result.Error);
        }
        else
        {
            foreach (var claim in result.User.Claims)
            {
                Console.WriteLine("{0}:{1}", claim.Type, claim.Value);
            }
        }
        http.Stop();

        return result.AccessToken;
    }
}