#nullable enable
using System.Text.Json.Serialization;


namespace Aplicacao_integracao.Models.Asana
{
    public sealed record ProjectResponse
    (
        [property: JsonPropertyName("data")] List<AsanaProject> Data
    );
    public sealed record AsanaProject
    (
        [property: JsonPropertyName("gid")] string Id,
        [property: JsonPropertyName("name")] string Name
    );
    public sealed record SectionRequest
    (
        [property: JsonPropertyName("data")] AsanaSection Section
    );
    public sealed record AsanaSection
    (
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("gid")] string? Id
    );
    public sealed record TaskRequest
    (
        [property: JsonPropertyName("data")] AsanaTask Task
    );
    public sealed record AsanaTask
    (
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("completed")] bool Completed,
        [property: JsonPropertyName("notes")] string Description,
        [property: JsonPropertyName("assignee_section")] string? Section,
        [property: JsonPropertyName("start_at")] DateTime Start,
        [property: JsonPropertyName("due_at")] DateTime Due,
        [property: JsonPropertyName("projects")] List<string> Projects
    );

    public sealed record TrelloBoard
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name
    );
    public sealed record TrelloList
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name
    );
    public sealed record TrelloCard
    (
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("closed")] bool Closed,
        [property: JsonPropertyName("desc")] string Description,
        [property: JsonPropertyName("assignee_section")] string Section,
        [property: JsonPropertyName("start")] DateTime Start,
        [property: JsonPropertyName("due")] DateTime Due
    );

    public sealed record Configuration
    (
        [property: JsonPropertyName("trelloUser")] string TrelloUser,
        [property: JsonPropertyName("trelloKey")] string TrelloKey,
        [property: JsonPropertyName("trelloToken")] string TrelloToken,
        [property: JsonPropertyName("asanaToken")] string AsanaToken,
        [property: JsonPropertyName("projectsMapper")] List<ProjectMapper> ProjectsMapper
    );

    public sealed record ProjectMapper
    (
        [property: JsonPropertyName("trelloBoard")] string TrelloBoard,
        [property: JsonPropertyName("asanaProject")] string AsanaProject
    );

    public sealed record ApiResponse
    (
        [property: JsonPropertyName("message")] string Message,
        [property: JsonPropertyName("success")] bool Success,
        [property: JsonPropertyName("tasksCreated")] int? TasksCreated = 0,
        [property: JsonPropertyName("listsCreated")] int? ListsCreated = 0
    );


}

