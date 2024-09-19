using System.Configuration;
using IdentityModel.OidcClient;
using System.Diagnostics;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;


namespace OidcAuth
{

    public class GetOidcAuth
    {

        private static ILogger _logger;

        static GetOidcAuth()
        {
            ApplicationLogging.ConfigureLogger();
            _logger = ApplicationLogging.CreateLogger<GetOidcAuth>() ?? NullLogger<GetOidcAuth>.Instance;

        }

        public static string GetUserDetails()
        {
            var accessToken = MainAsync().GetAwaiter().GetResult();
            _logger.LogInformation($"AccessToken: {accessToken}");
            return accessToken;
        }
        public static async Task<string> MainAsync()
        {
            var authority = ConfigurationManager.AppSettings["Authority"];
            var clientid = ConfigurationManager.AppSettings["ClientId"];
            var redirectUrl = ConfigurationManager.AppSettings["RedirectUrl"];

            _logger.LogInformation("Calling 1");

            var accessToken = await SignIn(authority, clientid, redirectUrl);
            return accessToken.ToString();
            //Console.WriteLine("===================================");
            //Console.WriteLine($"AccessToken: {accessToken}");
            //Console.WriteLine("===================================");
        }
        private static async Task<string> SignIn(string authority, string clientId, string redirectUrl)
        {
            // Create and configure the logger factory
            //var logFileName = ConfigurationManager.AppSettings["OidcClientLog"];
            //var loggerFactory = LoggerFactory.Create(builder =>
            //{
            //    builder.AddProvider(new FileLoggerProvider(logFileName));
            //});

            // Create and configure the logger factory
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddProvider(new FileLoggerProvider("OidcClientLog.txt"));
            });

            var http = new HttpListener();
            http.Prefixes.Add(redirectUrl);
            http.Start();


            var options = new OidcClientOptions
            {
                Authority = authority,
                ClientId = clientId,
                Scope = "openid profile api",
                RedirectUri = redirectUrl,
                LoggerFactory = loggerFactory,
            };

            var client = new OidcClient(options);
            //var state = await client.PrepareLoginAsync();

            AuthorizeState state = null;

            try
            {
                state = await client.PrepareLoginAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while preparing the login.");
                return null; // or handle the error as needed
            }

            //Console.WriteLine($"Start URL: {state.StartUrl}");

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
                //Console.WriteLine("Error", result.Error);
            }
            else
            {
                foreach (var claim in result.User.Claims)
                {
                    //Console.WriteLine("{0}:{1}", claim.Type, claim.Value);
                }
            }
            http.Stop();

            return result.AccessToken;
        }
    }
}