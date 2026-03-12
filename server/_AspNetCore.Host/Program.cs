/* ****************************** [NOTICE] *******************************
1. _AspNetCore.Host project is for testing, debugging and generating openapi definitions purposes only,
it is not used (and should not be used) as real hosting for any purposes as the WFAssist app is published as nuget package
and used via RegisterWfAssistApp extension on WebApplication.

2. WfAssist app uses Scalar app to expose api endpoints in client app, all _AspNetCore.Host host urls can be found in launchSettings.json
3. If you also want to run a WfAssist client app from this Host, build and copy WfAssist client binaries from /dist directory to "_AspNetCore.Host/bin/debug/net10.0/wwwroot/wfAssist"

app urls:
scalar - http://localhost:7128/scalar
wfassist - http://localhost:7128/wfassist (if binaries are in Host output)
************************************************************************ */

using WfAssist.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

builder.ConfigureWfAssistAppBuilder();

var app = builder.Build();
app.ConfigureWfAssistApp();

app.Run();
