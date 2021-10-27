using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using CSC_api.Services;
using Microsoft.Extensions.Options;
using CSC_api.Models;

namespace CSC_api
{
    public class Startup
    {
        private IConfiguration Configuration;

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.Configure<AzureStorageConfig>(Configuration.GetSection("AzureStorageConfig"));

            var StorageSettings = new AzureStorageConfig {
                StorageUri = "https://msdtables.table.core.windows.net/",
                StorageAccountName= "msdtables",
                StorageAccountKey= "NcAmtts1UZKS0vpgDmDmecWWo8/M811pcj+Nw2+Zoi/IfaxlJ/9II1JmdquE8XUYSjJxxGKg6nwWZkYwJ1T2dA=="
            };

            services.AddSingleton<IAzureTableService<Client>>(x =>
            {
                var options = x.GetRequiredService<IOptions<AzureStorageConfig>>();
                return new AzureTableService<Client>("MSDClients", options.Value);
            });
            


            services.AddSingleton<IClientService>(x =>
                new ClientService(x.GetRequiredService<IAzureTableService<Client>>())
                );
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
