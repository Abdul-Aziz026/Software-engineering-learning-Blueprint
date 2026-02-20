namespace Domain.Entities;

public class Chapter : BaseEntity
{
    public string SubjectId { get; set; } = string.Empty;
    public string ChapterName { get; set; } = string.Empty;
    public List<Lesson> Lessons { get; set; } = new();
}

public class Lesson
{
    public string Id { get; set; } = string.Empty;
    public string LessonName { get; set; } = string.Empty;
}

public class LessonDetails : BaseEntity
{
    public string LessonId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> ReferenceUrls { get; set; } = new();
}

