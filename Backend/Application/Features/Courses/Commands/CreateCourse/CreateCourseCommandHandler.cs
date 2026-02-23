
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.Courses.Commands.CreateCourse;

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, string>
{
    private readonly ICourseRepository _courseRepository;
    public CreateCourseCommandHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<string> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = new Subject
        {
            Name = request.Name,
            Description = request.Description
        };
        
        var added = await _courseRepository.AddAsync(course);
        if (!added) return string.Empty;
        return course.Id;
    }
}
