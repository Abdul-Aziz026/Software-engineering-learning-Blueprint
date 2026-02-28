using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Courses.Commands.DeleteCourse;


public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand>
{
    private readonly ICourseRepository _courseRepository;
    public DeleteCourseCommandHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }
    public async Task Handle(DeleteCourseCommand command, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync<Subject>(command.Id);
        if (course is null)
        {
            throw new NotFoundException();
        }

        // Cascade delete: Chapters and LessonDetails
        var chapters = await _courseRepository.GetItemsByConditionAsync<Chapter>(c => c.SubjectId == command.Id);
        if (chapters != null)
        {
            foreach (var chapter in chapters)
            {
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
                await _courseRepository.DeleteByIdAsync<Chapter>(chapter.Id);
            }
        }

        var deleted = await _courseRepository.DeleteByIdAsync<Subject>(command.Id);
        if (!deleted)
        {
            throw new UnknownException();
        }
    }
}
