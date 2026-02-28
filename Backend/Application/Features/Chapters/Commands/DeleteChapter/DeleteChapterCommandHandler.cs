using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Chapters.Commands.DeleteChapter;

public class DeleteChapterCommandHandler : IRequestHandler<DeleteChapterCommand>
{
    private readonly ICourseRepository _courseRepository;

    public DeleteChapterCommandHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task Handle(DeleteChapterCommand command, CancellationToken cancellationToken)
    {
        var chapter = await _courseRepository.GetByIdAsync<Chapter>(command.Id);
        if (chapter is null)
        {
            throw new NotFoundException();
        }

        // Cascade delete: LessonDetails associated with the embedded lessons
        if (chapter.Lessons != null)
        {
            foreach (var lesson in chapter.Lessons)
            {
                var lessonDetails = await _courseRepository.GetItemsByConditionAsync<LessonDetails>(ld => ld.LessonId == lesson.Id);
                if (lessonDetails != null)
                {
                    foreach (var detail in lessonDetails)
                    {
                        await _courseRepository.DeleteByIdAsync<LessonDetails>(detail.Id);
                    }
                }
            }
        }

        var deleted = await _courseRepository.DeleteByIdAsync<Chapter>(command.Id);
        if (!deleted)
        {
            throw new UnknownException();
        }
    }
}
