using Application.Features.Courses.Commands.CreateCourse;

namespace Application.Features.Courses.DTOs;

public class CreateCourseRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public CreateCourseCommand ToCreateCourseCommand()
    {
        return new CreateCourseCommand
        {
            Name = this.Name,
            Description = this.Description
        };
    }
}
