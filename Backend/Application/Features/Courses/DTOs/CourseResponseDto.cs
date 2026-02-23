
using Domain.Entities;

namespace Application.Features.Courses.DTOs;

public class CourseResponseDto
{
    public string Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public static class CourseResponseDtoExtensions
{
    public static CourseResponseDto ToCourseResponseDto(this Subject subject)
    {
        return new CourseResponseDto
        {
            Id = subject.Id,
            Name = subject.Name,
            Description = subject.Description
        };
    }
}
