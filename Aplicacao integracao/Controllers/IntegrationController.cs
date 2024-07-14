#nullable enable
using Aplicacao_integracao.Models.Trello;
using Aplicacao_integracao.Models.Asana;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Aplicacao_integracao.Controllers
{
    [ApiController]
    [Route("RunIntegration")]
    public class IntegrationController : ControllerBase
    {
        [HttpPost(Name = "RunIntegration")]
        public async Task<ApiResponse> RunAsync([FromBody] Configuration config)
        {
            try
            {
                List<TrelloBoard> boards = await Repository.GetBoardsAsync(config);
                ProjectResponse asanaProjectRepopnse = await Repository.GetProjectsAsync(config);
                var listResponse = 0;
                var taskResponse = 0;
                if (!boards.Any())
                {
                    return ErrorResponse(Error.BoardIsNull);
                }
                if (asanaProjectRepopnse == null || !asanaProjectRepopnse.Data.Any())
                {
                    return ErrorResponse(Error.ProjectsIsNull);
                }
                
                foreach (TrelloBoard board in boards)
                {
                    var boardMapper = config.ProjectsMapper.Find(x => string.Equals(x.TrelloBoard, board.Name));
                    if(boardMapper != null)
                    {
                        var asanaProjectMapper = asanaProjectRepopnse.Data.Find(x => string.Equals(x.Name, boardMapper.AsanaProject, StringComparison.OrdinalIgnoreCase));
                        if(asanaProjectMapper != null)
                        {
                            var trelloLists = await Repository.GetListsAsync(config, board.Id);

                            foreach (TrelloList list in trelloLists)
                            {
                                var trelloCards = await Repository.GetCardsAsync(config, list.Id);

                                var asanaSection = Mapper.ToAsanaSection(list);
                                var sectionResponse = await Repository.PostSectionsAsync(config, asanaProjectMapper.Id, asanaSection);
                                var isSuccess = sectionResponse.Item1;
                                var sectionResult = sectionResponse.Item2;

                                if (isSuccess)
                                {
                                    listResponse++;
                                    foreach (TrelloCard card in trelloCards)
                                    {
                                        var asanaTask = Mapper.ToAsanaTask(card, sectionResult.Section.Id, asanaProjectMapper.Id);
                                        var cardResponse = await Repository.PostTaskAsync(config, asanaProjectMapper.Id, asanaTask);
                                        if (cardResponse.IsSuccessStatusCode)
                                        {
                                            taskResponse++;
                                        }
                                    }
                                }

                            }
                        }
                        
                    }
                    
                }


                return new ApiResponse
                (
                    Message: Success.IntegrationSuccess,
                    Success: true,
                    TasksCreated: taskResponse,
                    ListsCreated: listResponse
                );
            } 
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message);
            }
            
        }

        private static ApiResponse ErrorResponse(string msg)
        {
            return new ApiResponse
            (
                Message: msg,
                Success: false
            );
        }
    }
}