using MediatR;

namespace Application.Features.Chapters.Commands.UpdateChapter;

public class UpdateChapterCommand : IRequest
{
    public string Id { get; set; } = string.Empty;
    public string ChapterName { get; set; } = string.Empty;
}
