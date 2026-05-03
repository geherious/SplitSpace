using SplitSpace.FinanceService.Dal;
using SplitSpace.FinanceService.Logic;
using SplitSpace.FinanceService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc().AddJsonTranscoding();;
builder.Services.AddGrpcReflection();

builder.Services.AddGrpcSwagger();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(type => type.FullName);
});

builder.Services.AddRepositories();
builder.Services.AddServices();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5002, o => o.Protocols =
        Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2);
    
    options.ListenAnyIP(5004, o => o.Protocols =
        Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGrpcService<FinanceServiceGrpc>();
app.MapGrpcService<SearchServiceGrpc>();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

using var scope = app.Services.CreateScope();
var migrator = scope.ServiceProvider.GetRequiredService<DatabaseMigrator>();
await migrator.MigrateAsync();

app.Run();
