using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CourseController : ControllerBase
{

    public CourseController()
    {
    }

    [HttpGet("{id}/structure")]
    public async Task<IActionResult> GetCourseStructure(string id)
    {
        // Dummy data for testing
        var dummyCourse = new
        {
            Id = 1,
            Title = "Complete Web Development Course",
            Chapters = new[]
            {
                    new
                    {
                        Id = 1,
                        Name = "Introduction to HTML",
                        Lessons = new[]
                        {
                            new { Id = 1, Title = "What is HTML?" },
                            new { Id = 2, Title = "HTML Tags and Elements" },
                            new { Id = 3, Title = "Creating Your First Page" }
                        }
                    },
                    new
                    {
                        Id = 2,
                        Name = "CSS Fundamentals",
                        Lessons = new[]
                        {
                            new { Id = 4, Title = "Introduction to CSS" },
                            new { Id = 5, Title = "Selectors and Properties" },
                            new { Id = 6, Title = "Box Model" },
                            new { Id = 7, Title = "Flexbox Layout" }
                        }
                    },
                    new
                    {
                        Id = 3,
                        Name = "JavaScript Basics",
                        Lessons = new[]
                        {
                            new { Id = 8, Title = "Variables and Data Types" },
                            new { Id = 9, Title = "Functions" },
                            new { Id = 10, Title = "DOM Manipulation" }
                        }
                    }
                }
        };

        return Ok(dummyCourse);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourse(string id)
    {
        // Dummy data for testing
        var dummyCourse = new
        {
            Id = 1,
            Title = "Complete Web Development Course",
            Description = "Learn HTML, CSS, and JavaScript from scratch",
            Duration = "40 hours",
            Level = "Beginner",
            Chapters = new[]
            {
                    new
                    {
                        Id = 1,
                        Name = "Introduction to HTML",
                        Lessons = new[] { new { Id = 1, Title = "What is HTML?" } }
                    },
                    new
                    {
                        Id = 2,
                        Name = "CSS Fundamentals",
                        Lessons = new[] { new { Id = 4, Title = "Introduction to CSS" } }
                    },
                    new
                    {
                        Id = 3,
                        Name = "JavaScript Basics",
                        Lessons = new[] { new { Id = 8, Title = "Variables and Data Types" } }
                    }
                }
        };

        return Ok(dummyCourse);

    }
}