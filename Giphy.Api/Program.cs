using Giphy.Application.Extensions;
using Giphy.Infrastructure.Extensions;
using Giphy.Persistance.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
var configuration = builder.Configuration;
services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddUseCaseDependencies(configuration)
    .AddPersistence(configuration)
    .AddInfrastructure(configuration);

services.AddHealthChecks()
    .AddPersistanceHealthChecks(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandler();

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(endpoints => { endpoints.MapHealthChecks("/health"); });

app.Run();