using System.Net;
using System.Reflection;
using System.Text.Json;
using Giphy.Application.Exceptions;
using Giphy.Application.Mapping;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace Giphy.Application.Extensions;

public static class DependencyExtensions
{
    public static IServiceCollection AddUseCaseDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(c => c.AsScoped(), Assembly.GetExecutingAssembly());
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        
        var redisString = configuration.GetConnectionString("Redis");
        services.AddSingleton(_ => RedLockFactory.Create(new List<RedLockMultiplexer> { ConnectionMultiplexer.Connect(redisString) }));
        return services;
    }

    public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        var errorApp = app.New();
        errorApp.Run(async context =>
        {
            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            var baseException = exceptionHandlerPathFeature.Error;
            var statusCode = (int)HttpStatusCode.BadRequest;
            var message = baseException.Message;
            object? data = null;
            if (baseException is StatusCodeException exception)
            {
                statusCode = exception.StatusCode;
                if (exception.Message != null!) message = exception.Message;
                data = exception.Data;
            }

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                error = string.IsNullOrWhiteSpace(message)
                    ? Enum.GetName(typeof(HttpStatusCode), statusCode)
                    : message,
                data
            }));
        });
        var exceptionHandlerPipeline = errorApp.Build();
        app.UseMiddleware<ExceptionHandlerMiddleware>(Options.Create(new ExceptionHandlerOptions
        {
            ExceptionHandler = exceptionHandlerPipeline
        }));
    }
}