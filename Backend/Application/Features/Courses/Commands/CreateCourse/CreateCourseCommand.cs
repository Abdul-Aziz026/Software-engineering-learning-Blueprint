
using Application.Features.Courses.DTOs;
using MediatR;

namespace Application.Features.Courses.Commands.CreateCourse;

public class CreateCourseCommand : IRequest<string>
{
    public string Name { get; set; }
    public string Description { get; set; }

    
}
