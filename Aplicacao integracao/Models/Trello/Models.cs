using System.Text.Json.Serialization;


namespace Aplicacao_integracao.Models.Trello
{
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
}

