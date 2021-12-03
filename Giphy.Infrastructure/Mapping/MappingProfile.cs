using AutoMapper;
using System.Reflection;
using Giphy.Application.Interfaces;

namespace Giphy.Infrastructure.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        AllowNullCollections = true;
        ApplyMappingsFromAssembly(Assembly.GetAssembly(typeof(MappingProfile))!);
    }

    private void ApplyMappingsFromAssembly(Assembly assembly)
    {
        var types = assembly.GetExportedTypes()
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAutoMap<,>)));

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);
            const string? methodName = "CreateMap";
            var methodInfo = type.GetMethod(methodName) ?? type.GetInterface("IAutoMap`2")!.GetMethod(methodName);

            methodInfo?.Invoke(instance, new object[] { this });
        }
    }
}