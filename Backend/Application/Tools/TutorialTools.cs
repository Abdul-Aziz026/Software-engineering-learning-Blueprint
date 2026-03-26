
using ModelContextProtocol.Server;
using System.ComponentModel;
namespace Application.Tools;

[McpServerToolType]
public class TutorialTools
{
    [McpServerTool]
    [Description("Generate a professional tutorial blog from beginner to expert level. Use when user asks to learn or explain any topic.")]
    public async Task<string> GenerateTutorialBlog(
        [Description("Topic or subject name (e.g., Redis, ASP.NET Core, Angular Signals)")]
        string topic,

        [Description("Target level: 10-year-old, beginner, intermediate, advanced, professional")]
        string level,

        [Description("Optional technology context (e.g., ASP.NET Core, Angular)")]
        string? techContext = null
    )
    {
        var prompt = BuildPrompt(topic, level, techContext);
        return prompt;
    }

    private string BuildPrompt(string topic, string level, string? techContext)
    {
        return $@"
You are a world-class technical educator and content writer.

Create a PROFESSIONAL BLOG POST.

---

Topic: {topic}
Level: {level}
Tech Context: {techContext}

---

Structure:
1. Title (SEO optimized)
2. Introduction (why important + simple analogy)
3. Explanation based on level
4. Step-by-step guide
5. Code examples (if applicable)
6. Real-world use cases
7. Common mistakes
8. Best practices
9. Summary

---

Rules:
- Markdown format
- Clear and structured
- Short paragraphs
- Blog-ready (Medium/Dev.to)

Return ONLY blog content.
";
    }
}
