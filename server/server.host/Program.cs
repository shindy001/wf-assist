using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Serilog;
using WfAssist.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Services.AddSerilog();
builder.Services.AddWfAssistServices();
// TODO - set custom json options in WfAssistApp to avoid possible conflict with default/user serializer options
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.AllowOutOfOrderMetadataProperties = true;
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.AllowTrailingCommas = true;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

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

await app.UseWfAssistApp(excludeFromOpenApi: false);

app.Run();