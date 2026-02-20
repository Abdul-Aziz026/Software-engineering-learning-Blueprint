using Application.Features.Courses.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CourseController : ControllerBase
{
    List<SubjectResponseDto> subjects = new()
    {
        new SubjectResponseDto
        {
            Id = "123",
            Name = "C#",
            Description = "C# Fundametals of beginneer to intermediate"
        },
        new SubjectResponseDto
        {
            Id = "1234",
            Name = "Asp.net Core",
            Description = "Asp.net Core Fundametals of beginneer to intermediate"
        },
        new SubjectResponseDto
        {
            Id = "12345",
            Name = "Docker File",
            Description = "Docker File Fundametals of beginneer to intermediate"
        },
        new SubjectResponseDto
        {
            Id = "123456",
            Name = "Angular",
            Description = "Angular Fundametals of beginneer to intermediate"
        },
        new SubjectResponseDto
        {
            Id = "1234567",
            Name = "Networking",
            Description = "Networking Fundametals of beginneer to intermediate"
        },
        new SubjectResponseDto
        {
            Id = "12345678",
            Name = "SQL",
            Description = "SQL Fundametals of beginneer to intermediate"
        },
        new SubjectResponseDto
        {
            Id = "123456789",
            Name = "MongoDB",
            Description = "MongoDB Fundametals of beginneer to intermediate"
        },
        new SubjectResponseDto
        {
            Id = "1234567890",
            Name = "Git",
            Description = "Git Fundametals of beginneer to intermediate"
        }
    };

    public CourseController()
    {
    }

    // GET: api/Subjects
    /// <summary>
    /// Get all subjects
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SubjectResponseDto>>> GetSubjects()
    {
        return Ok(subjects);
    }

    // GET: api/Subjects/5
    /// <summary>
    /// Get a specific subject by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<SubjectResponseDto>> GetSubject(string id)
    {
        var subject = subjects.Where(s => s.Id == id).FirstOrDefault();
        return Ok(subject);
    }

    // POST: api/Subjects
    /// <summary>
    /// Create a new subject
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult> CreateSubject(CreateSubjectDto createDto)
    {
        
        subjects.Add(new SubjectResponseDto
        {
            Id = createDto.Id,
            Name = createDto.Name,
            Description = createDto.Description
        });
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

        var subject = subjects.Where(s => s.Id == id).FirstOrDefault();
        subject.Name = updateDto.Name;
        subject.Description = updateDto.Description;

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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSubject(string id)
    {
        var subject = subjects.Where(s => s.Id == id).FirstOrDefault();
        if (subject == null)
        {
            return NotFound();
        }
        subjects.Remove(subject);

        return Ok(new
        {
            Message = "Subject deleted successfully",
        });
    }
}