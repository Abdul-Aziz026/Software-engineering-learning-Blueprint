
using Application.Features.Courses.Query.GetAllCourses;
using MassTransit;

namespace API.Extensions;

public static class MasstransitAndMediatRExtensions
{
    public static IServiceCollection AddMediatRAndMasstransit(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(GetAllCoursesQueryHandler).Assembly);
        });
        return services;
    }
}
