using Microsoft.Extensions.DependencyInjection.Extensions;
using SplitSpace.SpaceService.Dal;
using SplitSpace.SpaceService.Dal.Database;
using SplitSpace.SpaceService.Producing;
using SplitSpace.SpaceService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddGrpcReflection();

builder.Services.AddGrpcSwagger();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediator(options =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
});

builder.Services.AddRepositories();
builder.Services.AddClientFacades();
builder.Services.AddProducers();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5002, o => o.Protocols =
        Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2);

    options.ListenAnyIP(5004, o => o.Protocols =
        Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGrpcService<SpaceServiceGrpc>();
app.MapGrpcService<InvitationServiceGrpc>();
app.MapGrpcReflectionService();

using (var scope = app.Services.CreateScope())
{
    var migrator = scope.ServiceProvider.GetRequiredService<DatabaseMigrator>();
    await migrator.MigrateAsync();
}

app.Run();
