using System;
using Microsoft.OpenApi.Models;

namespace ApiEcommerce.Constants;

public class VersionInfo
{

    public static readonly OpenApiInfo v1 = new()
    {
        Version = "v1",
        Title = "API Ecommerce",
        Description = "API Ecommerce ASP.NET Core Web API",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Soporte API Ecommerce",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "API Ecommerce License",
            Url = new Uri("https://example.com/license")
        }
    };

    public static readonly OpenApiInfo v2 = new()
    {
        Version = "v2",
        Title = "API Ecommerce 2.0",
        Description = "API Ecommerce ASP.NET Core Web API",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Soporte API Ecommerce",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "API Ecommerce License",
            Url = new Uri("https://example.com/license")
        }
    };

}
