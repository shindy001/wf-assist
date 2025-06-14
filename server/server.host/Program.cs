using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using server.lib;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi(opt =>
{
    opt.AddDocumentTransformer((document, _, _) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "WFAssist API Reference",
            Description = "API for WFAssist client."
        };
        return Task.CompletedTask;
    });
});

var app = builder.Build();

app.UseWfAssistApp();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opt =>
    {
        opt.EnabledClients = [ScalarClient.Curl, ScalarClient.Axios, ScalarClient.Fetch];
        opt.EnabledTargets = [ScalarTarget.Shell, ScalarTarget.JavaScript];
    });
}

app.Run();