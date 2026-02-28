using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Chapters.Commands.UpdateChapter;

public class UpdateChapterCommandHandler : IRequestHandler<UpdateChapterCommand>
{
    private readonly ICourseRepository _courseRepository;

    public UpdateChapterCommandHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task Handle(UpdateChapterCommand request, CancellationToken cancellationToken)
    {
        var chapter = await _courseRepository.GetByIdAsync<Chapter>(request.Id);
        if (chapter == null)
        {
            throw new NotFoundException();
        }

        chapter.ChapterName = request.ChapterName;

        var updated = await _courseRepository.UpdateAsync(chapter);
        if (!updated)
        {
            throw new UnknownException();
        }

        return;
    }
}
