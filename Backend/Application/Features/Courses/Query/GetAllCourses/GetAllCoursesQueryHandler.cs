
using Application.Common.Interfaces.Repositories;
using Application.Features.Courses.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.Courses.Query.GetAllCourses;

public class GetAllCoursesQueryHandler : IRequestHandler<GetAllCoursesQuery, List<CourseResponseDto>>
{
    private readonly ICourseRepository _courseRepository;
    public GetAllCoursesQueryHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<List<CourseResponseDto>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
    {
        var courses = await _courseRepository.GetAllAsync<Subject>();
        var courseDtos = courses.Select(o => o.ToCourseResponseDto()).ToList();
        return courseDtos;
    }
}
