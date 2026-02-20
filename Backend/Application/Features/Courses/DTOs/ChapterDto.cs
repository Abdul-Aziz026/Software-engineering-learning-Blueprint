using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Courses.DTOs;

public class ChapterDto
{
    public string SubjectId { get; set; } = string.Empty;
    public string ChapterName { get; set; } = string.Empty;
    public List<Lesson> Lessons { get; set; } = new();
}
