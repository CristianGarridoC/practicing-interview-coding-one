using ApplicationCore.Interfaces;
using ApplicationCore.Persistence;
using ApplicationCore.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationCore;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationCore(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);
        //services.AddMediatR(typeof(DependencyInjection).Assembly);
        services.AddDbContext<ApplicationDbContext>(options => 
            options.UseInMemoryDatabase("InterviewCodingOne")
        );
        services.AddTransient<IPersonRepository, PersonRepository>();
        services.AddHttpClient("quote",options =>
        {
            options.BaseAddress = new Uri("http://api.forismatic.com/");
            options.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        services.AddHttpClient("picsum",options =>
        {
            options.BaseAddress = new Uri("https://picsum.photos/");
            options.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
        });
        return services;
    }
}