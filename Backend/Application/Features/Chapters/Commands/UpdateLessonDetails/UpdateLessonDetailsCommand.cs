using MediatR;
using System.Collections.Generic;

namespace Application.Features.Chapters.Commands.UpdateLessonDetails;

public class UpdateLessonDetailsCommand : IRequest
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> ReferenceUrls { get; set; } = new();
}
