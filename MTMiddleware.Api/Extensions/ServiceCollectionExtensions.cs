using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MTMiddleware.Data;
using MTMiddleware.Identity.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace MTMiddleware.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddAutoMapperX(this IServiceCollection services)
    {
        var mapper = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new AutoMapperProfile());
            mc.AddProfile(new AuthAutoMapperProfile());
        }).CreateMapper();

        services.AddSingleton(mapper);
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "MTMiddleware API",
                Version = "v1",
                Description = "MTMiddleware API Service"
            });

            // add JWT Authentication
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter JWT Bearer token **_only_**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer", // must be lower case
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {securityScheme, new string[] { }}
                    });


            //options.CustomSchemaIds(x => x.FullName);
            //options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);

        });

        return services;
    }

    public static IServiceCollection AddJWTAuthentication(this IServiceCollection services, string authUrl, string audience)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authUrl;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authUrl,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true
                };
            });

        return services;
    }

    public static CorsOptions ConfigureCorsPolicy(this CorsOptions corsOptions, string[] allowedOrigins)
    {
        if (allowedOrigins.Any())
        {
            corsOptions.AddPolicy("CORSAllowedOrigins",
                              corsBuilder => corsBuilder
                              .WithOrigins(allowedOrigins.ToArray())
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                             );
        }
        else
        {
            corsOptions.AddPolicy("CORSAllowedOrigins",
                          corsBuilder => corsBuilder
                                            .AllowAnyHeader()
                                            .AllowAnyMethod());

        }

        return corsOptions;
    }
}