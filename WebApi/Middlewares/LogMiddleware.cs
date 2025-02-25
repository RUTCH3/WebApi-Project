using System.Diagnostics;

namespace WebApi.Middlewares;

public class LogMiddleware{

    private readonly RequestDelegate next;

    private readonly ILogger logger;
    
    public LogMiddleware(RequestDelegate next, ILogger<LogMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task Invoke(HttpContext c)
    {
        var sw = new Stopwatch();
        sw.Start();
        await next.Invoke(c);
        // logger.LogDebug($"{c.Request.Path}.{c.Request.Method} took {sw.ElapsedMilliseconds}ms."
        //     + $" User: {c.User?.FindFirst("userId")?.Value ?? "unknown"}\n");

        string logMessage = $"[{DateTime.Now}]] {c.Request.Method} "
            + $"{c.Request.Path}.{c.Request.Method} took {sw.ElapsedMilliseconds}ms."
            + $" User: {c.User?.FindFirst("userId")?.Value ?? "unknown"}\n";

        await File.AppendAllTextAsync("requests.log", logMessage);
    }
}

public static partial class MiddlewareExtensions
{
    public static IApplicationBuilder UseLogMiddleware(this IApplicationBuilder build)
    {
        return build.UseMiddleware<LogMiddleware>();
    }
}