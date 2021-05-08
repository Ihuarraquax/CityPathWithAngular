using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityPathWithAngular.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CityPathWithAngular
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var osmImporter = services.GetRequiredService<OSMImporter>();

                try
                {
                    // osmImporter.GenerateIntersections();
                    // osmImporter.GeneratePlaces();
                    // osmImporter.AddToDatabase();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while importing Open Source Map. " + Environment.NewLine + ex.Message);
                    throw;
                }
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}