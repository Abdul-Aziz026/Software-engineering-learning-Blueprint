
using Application.Features.Courses.DTOs;
using MediatR;

namespace Application.Features.Courses.Query.GetAllCourses;

public class GetAllCoursesQuery : IRequest<List<CourseResponseDto>>
{
}
