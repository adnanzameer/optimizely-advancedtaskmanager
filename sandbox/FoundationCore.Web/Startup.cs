using System.Net;
using AdvancedTaskManager.Infrastructure.Configuration;
using EPiServer.Authorization;
using EPiServer.Cms.Shell;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Framework.Web.Resources;
using EPiServer.Labs.LinkItemProperty;
using EPiServer.Scheduler;
using EPiServer.Web.Routing;
using FoundationCore.Web.Business;
using FoundationCore.Web.Extensions;
using FoundationCore.Web.Helpers;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;

namespace FoundationCore.Web;

public class Startup
{
    private readonly IWebHostEnvironment _webHostingEnvironment;
    private readonly IConfiguration _configuration;
    private readonly long _maxRequestSize = 419430400;

    public Startup(IWebHostEnvironment webHostingEnvironment, IConfiguration configuration)
    {
        _webHostingEnvironment = webHostingEnvironment;
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        //Allow uploading large files to the CMS
        services.Configure<FormOptions>(x =>
        {
            x.MultipartBodyLengthLimit = _maxRequestSize;
        });

        if (_webHostingEnvironment.IsDevelopment())
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(_webHostingEnvironment.ContentRootPath, "App_Data"));

            services.Configure<SchedulerOptions>(options => options.Enabled = false);

            // UI
            services.Configure<ClientResourceOptions>(uiOptions =>
            {
                uiOptions.Debug = true;
            });
        }

        //Lowercase urls, add trailing slash
        services.Configure<RouteOptions>(o =>
        {
            o.AppendTrailingSlash = false;
            o.LowercaseUrls = true;
        });

        services.AddRazorPages();

        services.AddCmsAspNetIdentity<ApplicationUser>();

        //services.AddAdvancedTaskManager();
        services.AddAdvancedTaskManager(o =>
        {
            o.DeleteChangeApprovalTasks = true;
            //o.AddContentApprovalDeadlineProperty = true;
            o.DeleteContentApprovalDeadlineProperty = true;
        });

        services.AddMvc(options =>
        {
            options.CacheProfiles.Add("NoCache", new CacheProfile()
            {
                NoStore = true,
                Location = ResponseCacheLocation.None,
                Duration = 0
            });
        });

        services.AddResponseCaching();

        services.AddCms();
        services.AddFind();
        services.AddFoundationCore();
        services.AddAdminUserRegistration();
        services.AddEmbeddedLocalization<Startup>();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/util/Login";
        });

        // Required by Wangkanai.Detection
        services.AddDetection();
        services.AddHttpContextAccessor();
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromSeconds(10);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = _ => false;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services.AddLinkItemProperty();

        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(60);
        });

        services.AddHttpsRedirection(options =>
        {
            // For security reasons return HTTP 301 (Moved Permanently) instead of 307 (Moved Temporary).
            // This is because with 301 any POST request will transform in GET request, thus eliminating
            // potentially dangerous and unsecure (from HTTP-request) code.
            // Also use the standard HTTPS port 443 for non-development environments.
            options.RedirectStatusCode = (int)HttpStatusCode.MovedPermanently;
            options.HttpsPort = _webHostingEnvironment.IsDevelopment() ? 5001 : 443;
        });


        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            // Only loopback proxies are allowed by default.
            // Clear that restriction because forwarders are enabled by explicit
            // configuration.
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });
        services.AddHttpClient();

        services.AddTinyMceConfiguration();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins",
                builder =>
                {
                    builder.WithOrigins("http://bs-local.com:3000", "http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseForwardedHeaders();
        
        var options = new RewriteOptions()
            .Add(context =>
            {
                if (context.HttpContext.Request.Path.StartsWithSegments("/util", StringComparison.InvariantCultureIgnoreCase) ||
                    context.HttpContext.Request.Path.StartsWithSegments("/episerver", StringComparison.InvariantCultureIgnoreCase) ||
                    context.HttpContext.Request.Path.StartsWithSegments("/EPiServer.Forms", StringComparison.InvariantCultureIgnoreCase) ||
                    context.HttpContext.Request.Path.StartsWithSegments("/globalassets", StringComparison.InvariantCultureIgnoreCase) ||
                    context.HttpContext.Request.Path.StartsWithSegments("/modules", StringComparison.InvariantCultureIgnoreCase) ||
                    context.HttpContext.Request.Path.StartsWithSegments("/error", StringComparison.InvariantCultureIgnoreCase) ||
                    context.HttpContext.Request.Path.StartsWithSegments("/localization-admin", StringComparison.InvariantCultureIgnoreCase))
                {
                    context.Result = RuleResult.SkipRemainingRules;
                }
            })
            // .AddRedirectToHttpsPermanent() // Redirect to HTTPS. This is commented due to an existing standard DXP rule.
            // .AddRedirectToNonWwwPermanent() // Redirect to naked URL. This is commented due to an existing standard DXP rule and general non-www --> www best practice.
            .AddRedirect("(.*)/$", "$1", 301);

        app.UseRewriter(options);


        app.UseStatusCodePagesWithReExecute("/error/{0}");

        app.UseDetection();
        app.UseSession();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseRequestLocalization();

        // Redirect http to https
        app.UseHttpsRedirection();

        app.UseCors("AllowSpecificOrigins");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapContent();
            endpoints.MapRazorPages();
        });
    }
}
