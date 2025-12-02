using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using RestEase.HttpClientFactory;
using SFA.DAS.RoatpFinance.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpFinance.Web.Infrastructure.AutoMapper;
using SFA.DAS.RoatpFinance.Web.ModelBinders;
using SFA.DAS.RoatpFinance.Web.Services;
using SFA.DAS.RoatpFinance.Web.Settings;
using SFA.DAS.RoatpFinance.Web.StartupExtensions;
using SFA.DAS.RoatpFinance.Web.Validators;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using FluentValidation;
using Microsoft.Extensions.Primitives;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.DfESignIn.Auth.Enums;

namespace SFA.DAS.RoatpFinance.Web
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private const string Culture = "en-GB";

        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _env;
        private readonly ILogger<Startup> _logger;

        public IWebConfiguration ApplicationConfiguration { get; set; }

        public Startup(IConfiguration configuration, IHostEnvironment env, ILogger<Startup> logger)
        {
            _env = env;
            _logger = logger;
            
            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory());
#if DEBUG
            if (!configuration["EnvironmentName"]!.Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                config.AddJsonFile("appsettings.json", true)
                    .AddJsonFile("appsettings.Development.json", true);
            }
#endif
            config.AddEnvironmentVariables();

            if (!configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                config.AddAzureTableStorage(options =>
                    {
                        options.ConfigurationKeys = configuration["ConfigNames"]!.Split(",");
                        options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                        options.EnvironmentName = configuration["EnvironmentName"];
                        options.PreFixConfigurationKeys = false;
                    }
                );
            }

            _configuration = config.Build();
            ApplicationConfiguration = _configuration.Get<WebConfiguration>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false; // Default is true, make it false
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            AddAuthentication(services);

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(Culture);
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo(Culture) };
                options.RequestCultureProviders.Clear();
            });

            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                options.ModelBinderProviders.Insert(0, new StringTrimmingModelBinderProvider());
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            services.AddValidatorsFromAssemblyContaining<Startup>();

            services.AddSession(opt => { opt.IdleTimeout = TimeSpan.FromHours(1); });

            services.AddCache(ApplicationConfiguration, _env);
            services.AddDataProtection(ApplicationConfiguration, _env);

            AddAntiforgery(services);

            services.AddHealthChecks();

            services.AddApplicationInsightsTelemetry();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            ConfigureClients(services);
            MappingStartup.AddMappings();

            ConfigureDependencyInjection(services);
        }

        private void AddAuthentication(IServiceCollection services)
        {
            services.AddAndConfigureDfESignInAuthentication(_configuration,
                "SFA.DAS.AdminService.Web.Auth",
                typeof(CustomServiceRole),
                ClientName.RoatpServiceAdmin,
                "/SignOut",
                "");
        }

        private static void AddAntiforgery(IServiceCollection services)
        {
            services.AddAntiforgery(options => options.Cookie = new CookieBuilder() { Name = ".RoatpFinance.Staff.AntiForgery", HttpOnly = false });
        }

        private void ConfigureClients(IServiceCollection services)
        {
            services.AddRestEaseClient<IQnaApiClient>(ApplicationConfiguration.QnaApiAuthentication.ApiBaseAddress)
                .AddHttpMessageHandler(() =>
                    new InnerApiAuthenticationHeaderHandler(new AzureClientCredentialHelper(_configuration),
                        ApplicationConfiguration.QnaApiAuthentication.Identifier));

            services.AddRestEaseClient<IRoatpApplicationApiClient>(ApplicationConfiguration.RoatpApplicationApiAuthentication.ApiBaseAddress)
                .AddHttpMessageHandler(() =>
                    new InnerApiAuthenticationHeaderHandler(new AzureClientCredentialHelper(_configuration),
                        ApplicationConfiguration.RoatpApplicationApiAuthentication.Identifier));
        }

        private void ConfigureDependencyInjection(IServiceCollection services)
        {
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient(x => ApplicationConfiguration);

            services.AddTransient<ISearchTermValidator, SearchTermValidator>();
            services.AddTransient<IRoatpFinancialClarificationViewModelValidator, RoatpFinancialClarificationViewModelValidator>();
            services.AddTransient<IRoatpFinancialApplicationViewModelValidator, RoatpFinancialApplicationViewModelValidator>();

            services.AddTransient<ICsvExportService, CsvExportService>();

            DependencyInjection.ConfigureDependencyInjection(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseRequestLocalization();
            app.UseStatusCodePagesWithReExecute("/ErrorPage/{0}");
            app.UseSecurityHeaders();
            app.Use(async (context, next) =>
            {
                if (!context.Response.Headers.ContainsKey("X-Permitted-Cross-Domain-Policies"))
                {
                    context.Response.Headers.Append("X-Permitted-Cross-Domain-Policies", new StringValues("none"));
                }
                await next();
            });
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseHealthChecks("/health");
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
