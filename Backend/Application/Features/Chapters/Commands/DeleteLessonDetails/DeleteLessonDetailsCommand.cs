using MediatR;

namespace Application.Features.Chapters.Commands.DeleteLessonDetails;

public class DeleteLessonDetailsCommand : IRequest
{
    public string Id { get; set; } = string.Empty;
}
