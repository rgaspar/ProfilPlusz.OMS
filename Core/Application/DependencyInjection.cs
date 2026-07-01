using Application.Common.Behaviors;
using Application.Common.Configuration;
using Application.Common.Mappings;
using Application.Common.Repositories;
using Application.Common.Services.AnswerTemplateManager;
using Application.Common.Services.EmailManager;
using Application.Common.Services.ExcelImport;
using Application.Common.Services.Location;
using Application.Common.Services.Statistics;
using Application.Features.EmailCustomerRecommendation;
using Application.Features.EmailCustomerRecommendation.Settings;
using Application.Features.EmailPartnerRecommendation;
using Domain.Services.Email;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        //>>> AutoMapper
        services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

        //>>> FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        //>>> MediatR
        services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            x.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
            x.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        });

        services.AddScoped<ExcelImportService>();
        services.AddScoped<IAnswerTemplateService, AnswerTemplateService>();
        services.AddSingleton<IStateService, StateService>();
        services.AddScoped<IEmailCustomerRecommendationManager, EmailCustomerRecommendationManager>();
        services.AddScoped<IEmailParserService, EmailParserService>();
        services.AddScoped<IEmailStatisticsService, EmailStatisticsService>();

        services.Configure<TemplateSettings>(configuration.GetSection("EmailTemplates"));
        services.Configure<EmailCustomerRecommendationSettings>(configuration.GetSection("EmailPartnerRecommendation"));

        //>>> Register services in Application.Features
        var assembly = Assembly.GetExecutingAssembly();
        var excelRowMapperType = typeof(IExcelRowMapper<>);
        var featureTypes = assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract)
            .Where(type => type.Namespace != null && type.Namespace.StartsWith("Application.Features"))
            .Where(type => !type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == excelRowMapperType));

        foreach (var type in featureTypes)
        {
            var interfaces = type.GetInterfaces();
            foreach (var serviceInterface in interfaces)
            {
                services.AddScoped(serviceInterface, type);
            }
            if (!interfaces.Any())
            {
                services.AddScoped(type);
            }
        }

        return services;
    }
}

