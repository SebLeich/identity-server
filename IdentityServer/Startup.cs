using IdentityServer.Entities;
using IdentityServer.Helpers;
using IdentityServer.Services;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer
{
    /// <summary>
    /// this class is defining the startup actions
    /// </summary>
    public class Startup
    {
        private readonly IConfiguration  _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// this method is called on startup for service configuration (e.g. dependency injection)
        /// </summary>
        /// <param name="services">service collection</param>
        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Startup)
                .GetTypeInfo()
                .Assembly
                .GetName()
                .Name;

            ConnectionStringSettings connectionStringSettings = _configuration
                .GetSection("ConnectionStrings")
                .Get<ConnectionStringSettings>();

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<IDSDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });

            services.AddCors(setup =>
            {
                setup.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.AllowAnyOrigin();
                });
            });

            services.AddDbContext<IDSDbContext>(options =>
                options.UseSqlServer(connectionStringSettings.DbConnection));

            services
                .AddIdentityServer(options =>
                {
                    options.Events.RaiseSuccessEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseErrorEvents = true;
                })
                .AddOperationalStore(options =>
                {
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30;
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionStringSettings.DbConnection, sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionStringSettings.DbConnection, sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddAspNetIdentity<User>()
                .AddProfileService<ProfileService>()
                .AddDeveloperSigningCredential();

            services.AddControllersWithViews();
            services.AddLocalApiAuthentication(principal =>
            {
                principal.Identities.First().AddClaim(new Claim("additional_claim", "additional_value"));

                return Task.FromResult(principal);
            });

            var cors = new DefaultCorsPolicyService(new LoggerFactory().CreateLogger<DefaultCorsPolicyService>())
            {
                AllowAll = true
            };
            services.AddSingleton<ICorsPolicyService>(cors);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();
            app.UseRouting();

            app.UseDeveloperExceptionPage();

            app.UseAuthorization();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

            app.UseCors();

            InitializeDatabase(app);
        }

        /// <summary>
        /// this method initializes the database and seeds the initial clients defined in the configuration
        /// </summary>
        /// <param name="app">application builder</param>
        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (
                var serviceScope = app
                    .ApplicationServices
                    .GetService<IServiceScopeFactory>()
                    .CreateScope()
            ){
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope
                    .ServiceProvider
                    .GetRequiredService<ConfigurationDbContext>();

                context.Database.Migrate();

                var changedClients = 0;

                foreach (var client in Configuration.Clients.Get())
                {
                    var persistedObject = context.Clients.Where(x => x.ClientId == client.ClientId).FirstOrDefault();
                    if(persistedObject == null)
                    {
                        Log.Logger.Information($"add new client: {client.ClientId}");
                        context.Clients.Add(client.ToEntity());
                        changedClients++;
                    }
                }

                if(changedClients > 0) context.SaveChanges();

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Configuration.Resources.IdentityResources)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var resource in Configuration.Resources.ApiScopes)
                    {
                        context.ApiScopes.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Configuration.Resources.ApiResources)
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
