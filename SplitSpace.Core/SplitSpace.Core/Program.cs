using Dapper;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using SplitSpace.Auth;
using SplitSpace.Finances;
using SplitSpace.SharedKernel.Database;
using SplitSpace.Spaces;
using SplitSpace.SharedKernel.Domain.Models;
using SplitSpace.SharedKernel.Domain;

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

SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());

AuthModule.Add(builder.Services, builder.Configuration);
FinancesModule.Add(builder.Services);
SpacesModule.Add(builder.Services);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5002, o => o.Protocols = HttpProtocols.Http2);

    options.ListenAnyIP(5004, o => o.Protocols = HttpProtocols.Http1);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

AuthModule.MapGrpc(app);
FinancesModule.MapGrpc(app);
SpacesModule.MapGrpc(app);
app.MapGrpcReflectionService();

await AuthModule.RunMigrations(app.Services);
await FinancesModule.RunMigrations(app.Services);
await SpacesModule.RunMigrations(app.Services);

app.Run();

public partial class Program
{
}
