using DataImportApi;
using DataImportHelper;
using GenericHelper;
using Microsoft.AspNetCore.Builder;

//var builder = Host.CreateApplicationBuilder(args);
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ISettings, Settings>();

builder.Services.AddHostedService<Worker>();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDataImport, DataImport>();

var host = builder.Build();

host.UseSwagger();
host.UseSwaggerUI();

host.MapControllers();
host.Run();
