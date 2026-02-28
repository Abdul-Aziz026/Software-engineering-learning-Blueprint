using MediatR;

namespace Application.Features.Chapters.Commands.DeleteChapter;

public class DeleteChapterCommand : IRequest
{
    public string Id { get; set; } = string.Empty;
}
