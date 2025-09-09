using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using WfAssist.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWfAssistServices();

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllCorsPolicy", policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAllCorsPolicy");
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Axios);
        options.EnabledClients = [ScalarClient.Curl, ScalarClient.Axios];
        options.EnabledTargets = [ScalarTarget.Shell, ScalarTarget.JavaScript];

    });
}

app.UseWfAssistApp(excludeFromOpenApi: false);

app.Run();