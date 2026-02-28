using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Chapters.Commands.UpdateLessonDetails;

public class UpdateLessonDetailsCommandHandler : IRequestHandler<UpdateLessonDetailsCommand>
{
    private readonly ICourseRepository _courseRepository;

    public UpdateLessonDetailsCommandHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task Handle(UpdateLessonDetailsCommand request, CancellationToken cancellationToken)
    {
        var lessonDetails = await _courseRepository.GetByIdAsync<LessonDetails>(request.Id);
        if (lessonDetails == null)
        {
            throw new NotFoundException();
        }

        lessonDetails.Title = request.Title;
        lessonDetails.Description = request.Description;
        lessonDetails.ReferenceUrls = request.ReferenceUrls;

        var updated = await _courseRepository.UpdateAsync(lessonDetails);
        if (!updated)
        {
            throw new UnknownException();
        }
    }
}
