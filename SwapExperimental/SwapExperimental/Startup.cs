using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swap.WebApi.Entities;
using Swap.WebApi.Repositories;
using Swap.WebApi.Repositories.Interfaces;
using Swap.WebApi.Repository;
using Swap.WebApi.SignalRChat.Hubs;

namespace Swap
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
            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                authOptions.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
            }).AddIdentityServerAuthentication(options =>
            {
                options.RequireHttpsMetadata = false;
                options.Authority = Configuration.GetSection("Addresses").GetValue<string>("AuthServer");
                options.ApiName = "Swap";
            });
            services.AddSingleton(Configuration);
            services.Configure<IConfiguration>(Configuration);

            services.AddMemoryCache();

            services.AddScoped<Database>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<ITradeRepository, TradeRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IInstantMessageRepository, InstantMessageRepository>();
            services.AddScoped<IUserToGroupRepository, UserToGroupRepository>();

            services.AddSignalR();

            services.AddDbContext<SwapContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("MainServer")));

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
            app.UseFileServer();
            app.UseDeveloperExceptionPage();
            app.UseDatabaseErrorPage();
            app.UseDatabaseErrorPage();
            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chatHub");
            });
            app.UseMvc(ConfigureRoutes);
        }

        private void ConfigureRoutes(IRouteBuilder routerBuilder)
        {
            routerBuilder.MapRoute("Default", "{controller=Home}/{action=Index}");
        }
    }
}
