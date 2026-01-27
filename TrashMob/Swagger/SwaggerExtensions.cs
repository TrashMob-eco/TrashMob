using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Linq;

namespace TrashMob.Swagger;

public static class SwaggerExtensions
{
    public static void AddTrashMobSwagger(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });
        serviceCollection.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
        });
        serviceCollection.AddSwaggerGen(options =>
        {
            options.OperationFilter<HideApiVersionParameter>();
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "trashmobapi v1", Version = "v1" });
            options.SwaggerDoc("v2", new OpenApiInfo { Title = "trashmobapi v2", Version = "v2" });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("bearer", document)] = []
            });

            options.TagActionsBy(api =>
            {
                if (api.ActionDescriptor is Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor descriptor)
                {
                    var name = descriptor.ControllerTypeInfo.Name;
                    if (name.Contains("ControllerV"))
                    {
                        return [name[..name.IndexOf("ControllerV")]];
                    }
                    return [descriptor.ControllerName];
                }
                return ["Other"];
            });

            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                if (apiDesc.ActionDescriptor is not Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor controller)
                    return false;

                var versions = controller.ControllerTypeInfo
                    .GetCustomAttributes(true)
                    .OfType<ApiVersionAttribute>()
                    .SelectMany(attr => attr.Versions)
                    .ToList();

                if (!versions.Any())
                {
                    return docName == "v1";
                }

                return versions.Any(v => $"v{v.MajorVersion}" == docName);
            });


            // Ensure documentation can be read by Swagger 
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic);

            foreach (var assembly in assemblies)
            {
                var xmlFile = $"{assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            }
        });
    }
}

public class HideApiVersionParameter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null) return;

        for(int i = 0; i < operation.Parameters.Count; i++)
        {
            var op = operation.Parameters[i];
            if(op.Name.Equals("api-version", StringComparison.OrdinalIgnoreCase))
            {
                operation.Parameters.RemoveAt(i);
                return;
            }
        }
    }
}