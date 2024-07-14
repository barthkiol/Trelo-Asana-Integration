using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System.Net.Mime;
using System.Text;
using Aplicacao_integracao.Models.Trello;
using Aplicacao_integracao.Models.Asana;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Aplicacao_integracao
{
    internal static class Repository
    {
        internal static async Task<List<TrelloBoard>> GetBoardsAsync(Configuration config)
        {
            HttpClient client = new HttpClient();

            var queryString = QueryString
               .Create("key", config.TrelloKey)
               .Add("token", config.TrelloToken);

            var uri = new Uri($"https://api.trello.com/1/members/{config.TrelloUser}/boards{queryString}");
          
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);

            var response = await client.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            var boards = JsonSerializer.Deserialize<List<TrelloBoard>>(responseContent);
            
            return boards;
        }

        internal static async Task<List<TrelloList>> GetListsAsync(Configuration config, string id)
        {
            HttpClient client = new HttpClient();

            var queryString = QueryString
               .Create("key", config.TrelloKey)
               .Add("token", config.TrelloToken);

            var uri = new Uri($"https://api.trello.com/1/boards/{id}/lists{queryString}");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);

            var response = await client.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            var lists = JsonSerializer.Deserialize<List<TrelloList>>(responseContent);

            return lists;
        }

        internal static async Task<List<TrelloCard>> GetCardsAsync(Configuration config, string id)
        {
            HttpClient client = new HttpClient();

            var queryString = QueryString
                .Create("key", config.TrelloKey)
                .Add("token", config.TrelloToken);

            var uri = new Uri($"https://api.trello.com/1/lists/{id}/cards{queryString}");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);

            var response = await client.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            var cards = JsonSerializer.Deserialize<List<TrelloCard>>(responseContent);

            return cards;
        }

        internal static async Task<ProjectResponse> GetProjectsAsync(Configuration config)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.AsanaToken);

            var uri = new Uri($"https://app.asana.com/api/1.0/projects");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);

            var response = await client.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            var projectResponse = JsonSerializer.Deserialize<ProjectResponse>(responseContent);

            return projectResponse;
        }

        internal static async Task<(bool,SectionRequest)> PostSectionsAsync(Configuration config, string id, AsanaSection section)
        {
            
            var sectionRequest = new SectionRequest(section);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.AsanaToken);

            var uri = new Uri($"https://app.asana.com/api/1.0/projects/{id}/sections");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uri);
            httpRequest.Content = new StringContent(JsonSerializer.Serialize(sectionRequest),Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await client.SendAsync(httpRequest);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var sectionResponse = JsonSerializer.Deserialize<SectionRequest>(responseContent);
                if(sectionResponse != null)
                {
                    return (true, sectionResponse);
                }
            }

            return (false, new SectionRequest(null));

        }

        internal static async Task<HttpResponseMessage> PostTaskAsync(Configuration config, string id, AsanaTask task)
        {
            var taskRequest = new TaskRequest(task);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.AsanaToken);

            var uri = new Uri($"https://app.asana.com/api/1.0/tasks");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uri);
            httpRequest.Content = new StringContent(JsonSerializer.Serialize(taskRequest));
            var response = await client.SendAsync(httpRequest);

            return response;
        }

    }
}
