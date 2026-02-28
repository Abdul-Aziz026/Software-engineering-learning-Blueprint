using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Chapters.Commands.DeleteLessonDetails;

public class DeleteLessonDetailsCommandHandler : IRequestHandler<DeleteLessonDetailsCommand>
{
    private readonly ICourseRepository _courseRepository;

    public DeleteLessonDetailsCommandHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task Handle(DeleteLessonDetailsCommand command, CancellationToken cancellationToken)
    {
        var lessonDetails = await _courseRepository.GetByIdAsync<LessonDetails>(command.Id);
        if (lessonDetails is null)
        {
            throw new NotFoundException();
        }
        
        string lessonIdToRemove = lessonDetails.LessonId;

        // Cascade delete: Remove the embedded Lesson representation from the parent Chapter
        var chapters = await _courseRepository.GetItemsByConditionAsync<Chapter>(c => c.Lessons.Any(l => l.Id == lessonIdToRemove));
        if (chapters != null)
        {
            foreach (var chapter in chapters)
            {
                var lessonToRemove = chapter.Lessons.FirstOrDefault(l => l.Id == lessonIdToRemove);
                if (lessonToRemove != null)
                {
                    chapter.Lessons.Remove(lessonToRemove);
                    await _courseRepository.UpdateAsync(chapter);
                }
            }
        }

        var deleted = await _courseRepository.DeleteByIdAsync<LessonDetails>(command.Id);
        if (!deleted)
        {
            throw new UnknownException();
        }
    }
}
