using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swap.AuthServer.IdentityServerConfiguration;
using Swap.WebApi.Entities;
using Swap.WebApi.Repositories.Interfaces;
using Swap.WebApi.Repositories;

namespace Swap.AuthServer
{
    public class Startup
    {
        private IConfiguration Configuration { get; set; }

        public Startup(IHostingEnvironment environment)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.SetBasePath(environment.ContentRootPath);
            builder.AddJsonFile("appsettings.json");
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityServer()
            .AddSigningCredential(new X509Certificate2(
                Configuration.GetSection("Addresses").GetValue<string>("RSA"), "password"))
            .AddInMemoryApiResources(InMemoryConfiguration.ApiResources())
            .AddInMemoryClients(InMemoryConfiguration.Clients())
            .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();

            services.AddTransient<IUserRepository, UserRepository>();

            services.AddDbContext<SwapContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("MainServer")));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseIdentityServer();
            app.UseStaticFiles();
        }
    }
}
