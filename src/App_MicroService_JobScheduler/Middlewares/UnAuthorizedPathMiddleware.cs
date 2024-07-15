using Microsoft.AspNetCore.Authentication;

namespace App_MicroService_JobScheduler.Middlewares;

public class UnAuthorizedPathMiddleware
{
    private readonly RequestDelegate _next;

    public UnAuthorizedPathMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var response = context.Response;
        var userIsAuthenticated = context.User?.Identity?.IsAuthenticated ?? false;

        var requestIsInSwagger = context.Request?.Path.Value?.Contains("/swagger") ?? false;

        if (response.StatusCode == (int)HttpStatusCode.Unauthorized || (!userIsAuthenticated && requestIsInSwagger))
        {
            //response.Redirect("/auth/login");
            await context.ChallengeAsync();
            return;
        }
        else
        {
            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }

    }
}
