using server.lib;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWfAssistApp();

app.Run();