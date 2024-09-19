using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OidcAuth
{
    
        class Program
        {
            static void Main(string[] args)
            {
                var iniReader = new IniFileReader("config.ini");

                string appName = iniReader.GetValue("General", "AppName", "DefaultAppName");
                string version = iniReader.GetValue("General", "Version", "1.0.0");
                string connectionString = iniReader.GetValue("Database", "ConnectionString", "DefaultConnectionString");

                Console.WriteLine($"AppName: {appName}");
                Console.WriteLine($"Version: {version}");
                Console.WriteLine($"ConnectionString: {connectionString}");
            }
        }
    }


