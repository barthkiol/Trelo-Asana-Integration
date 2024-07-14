#nullable enable
using Aplicacao_integracao.Models.Asana;
using Aplicacao_integracao.Models.Trello;

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

        public static AsanaTask ToAsanaTask(TrelloCard card, string section, string projects)
        {
            return new AsanaTask
            (
               Name: card.Name,
               Description: card.Description,
               Completed: card.Closed,
               Due: card.Due,
               Start: card.Start,
               Section: card.Section,
               Projects: new List<string> { projects }
            );
        }
    }
}
