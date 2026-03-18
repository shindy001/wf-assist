using System.Data.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Shared;

namespace WfAssist.AspNetCore;

internal sealed class TransactionMiddleware
{
    private static readonly PathString ApiRouteSegment = new($"/{Constants.ApiRoute}");
    private readonly RequestDelegate _next;

    public TransactionMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext httpContext, IDbConnectionProvider dbConnection)
    {
        if (!IsApiRoute(httpContext.Request.Path)
            || !IsSideEffect(httpContext.Request.Method))
        {
            await _next(httpContext);
            return;
        }

        DbTransaction? transaction = null;
        try
        {
            transaction = await dbConnection.DbConnection.BeginTransactionAsync();

            await _next(httpContext);

            await transaction.CommitAsync();
        }
        catch
        {
            if (transaction != null)
            {
                await transaction.RollbackAsync();
            }
            throw;
        }
        finally
        {
            if (transaction != null)
            {
                await transaction.DisposeAsync();
            }
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
        return requestPath.StartsWithSegments(ApiRouteSegment);
    }
}

public static class TransactionMiddlewareExtensions
{
    public static void UseTransactionMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<TransactionMiddleware>();
}