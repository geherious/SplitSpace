using SplitSpace.AuthService.Dal;
using SplitSpace.AuthService.Logic;
using SplitSpace.AuthService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc().AddJsonTranscoding();;
builder.Services.AddGrpcReflection();

builder.Services.AddGrpcSwagger();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDatabase();
builder.Services.AddServiceOptions(builder.Configuration);
builder.Services.AddServices();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000, o => o.Protocols =
        Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2AndHttp3);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGrpcService<AuthServiceGrpc>();
app.MapGrpcService<UserServiceGrpc>();
app.MapGrpcReflectionService();

using var scope = app.Services.CreateScope();
var migrator = scope.ServiceProvider.GetRequiredService<DatabaseMigrator>();
await migrator.MigrateAsync();

app.Run();