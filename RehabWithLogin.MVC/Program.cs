﻿using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace RehabWithLogin.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseSetting("detailedErrors", "true")
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}