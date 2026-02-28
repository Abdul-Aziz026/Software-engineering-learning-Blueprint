
using Application.Features.Courses.DTOs;
using MediatR;

namespace Application.Features.Lessons.Command.CreateLesson;

public class CreateLessonCommand : IRequest
{
    public string SubjectId { get; set; } = string.Empty;
    public string ChapterId { get; set; } = string.Empty;
    public string ChapterName { get; set; } = string.Empty;
    public LessonDto Lesson { get; set; } = new();
}
