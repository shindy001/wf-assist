using System.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace WfAssist.Workflows.Infrastructure.Middleware;

public sealed class TransactionMiddleware
{
    private readonly RequestDelegate _next;

    public TransactionMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext httpContext, IDbConnectionProvider dbConnectionProvider)
    {
        if (!IsApiRoute(httpContext.Request.Path)
            || !IsSideEffect(httpContext.Request.Method))
        {
            await _next(httpContext);
            return;
        }

        IDbTransaction? transaction = null;
        try
        {
            transaction = dbConnectionProvider.DbConnection.BeginTransaction();

            await _next(httpContext);

            transaction.Commit();
        }
        catch
        {
            transaction?.Rollback();
            throw;
        }
        finally
        {
            transaction?.Dispose();
        }
    }

    private static bool IsSideEffect(string httpMethod)
    {
        return HttpMethods.IsPost(httpMethod)
               || HttpMethods.IsPut(httpMethod)
               || HttpMethods.IsPatch(httpMethod)
               || HttpMethods.IsDelete(httpMethod);
    }

    private static bool IsApiRoute(PathString requestPath)
    {
        return requestPath.StartsWithSegments(Constants.ApiRouteSegment);
    }
}

public static class TransactionMiddlewareExtensions
{
    public static void UseTransactionMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<TransactionMiddleware>();
}