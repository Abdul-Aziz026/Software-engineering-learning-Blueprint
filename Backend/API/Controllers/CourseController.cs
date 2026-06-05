using Application.Common.Interfaces.Publisher;
using Application.Features.Courses.Commands.CreateCourse;
using Application.Features.Courses.Commands.DeleteCourse;
using Application.Features.Courses.Commands.UpdateCourse;
using Application.Features.Courses.DTOs;
using Application.Features.Courses.Query.GetAllCourses;
using Application.Features.Courses.Query.GetCourseById;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CourseController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    public CourseController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    // GET: api/Subjects
    /// <summary>
    /// Get all subjects
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CourseResponseDto>>> GetSubjects()
    {
        var query = new GetAllCoursesQuery();
        var subjects = await _messageBus.SendAsync<GetAllCoursesQuery, List<CourseResponseDto>>(query);
        return Ok(subjects);
    }

    // GET: api/Subjects/5
    /// <summary>
    /// Get a specific subject by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<CourseResponseDto>> GetSubject(string id)
    {
        var query = new GetCourseByIdQuery
        {
            CourseId = id
        };
        var response = await _messageBus.SendAsync<GetCourseByIdQuery, CourseResponseDto>(query);
        return Ok(response);
    }

    // POST: api/Subjects
    /// <summary>
    /// Create a new subject
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult> CreateSubject(CreateCourseRequestDto createDto)
    {
        var command = createDto.ToCreateCourseCommand();
        var subjectId = await _messageBus.SendAsync<CreateCourseCommand, string>(command);
        if (string.IsNullOrEmpty(subjectId))
        {
            return BadRequest("Failed to create subject");
        }
        return Created();
    }

    // PUT: api/Subjects/5
    /// <summary>
    /// Update an existing subject
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateSubject(string id, UpdateSubjectDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = new UpdateCourseCommand
        {
            Id = id,
            Name = updateDto.Name,
            Description = updateDto.Description
        };

        await _messageBus.SendAsync<UpdateCourseCommand>(command);

        return Ok(new
        {
            Message = "Subject updated successfully",
        });
    }

    // DELETE: api/Subjects/5
    /// <summary>
    /// Delete a subject
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteSubject(string id)
    {
        var command = new DeleteCourseCommand {
            Id = id 
        };
        await _messageBus.SendAsync(command);
        return NoContent();
    }
}