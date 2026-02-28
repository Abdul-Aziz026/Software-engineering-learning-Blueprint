using Application.Features.Chapters.Commands.CreateChapter;
using Application.Features.Lessons.Command.CreateLesson;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Courses.DTOs;

public class ChapterDto
{
    public string Id { get; set; } = string.Empty;
    public string SubjectId { get; set; } = string.Empty;
    public string ChapterName { get; set; } = string.Empty;
    public List<LessonDto> Lessons { get; set; } = new();

    public CreateChapterCommand ToCreateChapterCommand()
    {
        return new CreateChapterCommand
        {
            ChapterId = this.Id,
            SubjectId = this.SubjectId,
            ChapterName = this.ChapterName,
            Lesson = this.Lessons[0] ?? new LessonDto()
        };
    }

    public CreateLessonCommand ToCreateLessonCommand()
    {
        return new CreateLessonCommand
        {
            ChapterId = this.Id,
            SubjectId = this.SubjectId,
            ChapterName = this.ChapterName,
            Lesson = this.Lessons[this.Lessons.Count - 1] ?? new LessonDto()
        };
    }
}

public class LessonDto
{
    public string Id { get; set; } = string.Empty;
    public string LessonName { get; set; } = string.Empty;
}
