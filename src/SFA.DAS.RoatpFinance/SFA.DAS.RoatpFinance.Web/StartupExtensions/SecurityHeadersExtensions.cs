namespace SFA.DAS.RoatpFinance.Web.StartupExtensions;

public static class SecurityHeadersExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            context.Response.Headers.XFrameOptions = "SAMEORIGIN";
            context.Response.Headers.XXSSProtection = "1; mode=block";
            context.Response.Headers.XContentTypeOptions = "nosniff";
            context.Response.Headers.ContentSecurityPolicy = "default-src 'self'; img-src 'self' *.azureedge.net *.google-analytics.com; script-src 'self' 'unsafe-inline' *.azureedge.net *.googletagmanager.com *.google-analytics.com *.googleapis.com; style-src-elem 'self' *.azureedge.net; style-src 'self' *.azureedge.net; font-src 'self' *.azureedge.net data:;";
            context.Response.Headers.Referer = "strict-origin-when-cross-origin";
            context.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
            context.Response.Headers.Pragma = "no-cache";
            context.Response.Headers["X-Permitted-Cross-Domain-Policies"] = "none";
            await next();
        });

        return app;
    }
}