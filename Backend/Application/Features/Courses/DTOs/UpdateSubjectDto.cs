using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Courses.DTOs;

public class UpdateSubjectDto
{
    public string Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
