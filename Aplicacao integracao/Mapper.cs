#nullable enable
using Aplicacao_integracao.Models;

namespace Aplicacao_integracao
{
    public static class Mapper
    {
        public static AsanaSection ToAsanaSection(TrelloList list)
        {
            return new AsanaSection
            (
                Id: null,
                Name: list.Name
            );
        }

        public static AsanaTask ToAsanaTask(TrelloCard card, string member, string list, string projects)
        {
            return new AsanaTask
            (
               Name: card.Name,
               Description: card.Description,
               Completed: card.Closed,
               Due: card.Due,
               Member: string.IsNullOrWhiteSpace(member) ? null : member,
               Start: card.Start,
               Section: list,
               Projects: new List<string> { projects }
            );
        }
    }
}
