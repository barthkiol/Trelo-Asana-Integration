#nullable enable
using Aplicacao_integracao.Models;
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
                    var trelloMembers = await Repository.GetTrelloMembersAsync(config, board.Id);
                    var asanaMembers = await Repository.GetAsanaMembersAsync(config);
                    var asanaMemberId = "";

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
                                        asanaMemberId = GetAsanaMemberId(config, trelloMembers, asanaMembers, asanaMemberId, card);

                                        var asanaTask = Mapper.ToAsanaTask(card, asanaMemberId, sectionResult.Section.Id, asanaProjectMapper.Id);
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

        private static string GetAsanaMemberId(Configuration config, List<TrelloMember> trelloMembers, AsanaMemberResponse asanaMembers, string asanaMemberId, TrelloCard card)
        {
            var trelloMember = trelloMembers.Find(x => string.Equals(x.Id, card.Members.FirstOrDefault(), StringComparison.OrdinalIgnoreCase));
            if (trelloMember != null)
            {
                var mappedMember = config.MemberMapper.Find(x => string.Equals(x.TrelloMember, trelloMember.Name, StringComparison.OrdinalIgnoreCase));
                if (mappedMember != null)
                {
                    var asanaMember = asanaMembers.Data.Find(x => string.Equals(x.Name, mappedMember.AsanaMember, StringComparison.OrdinalIgnoreCase));
                    if (asanaMember != null)
                    {
                        asanaMemberId = asanaMember.Id;
                    }
                }
            }

            return asanaMemberId;
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