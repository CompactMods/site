using System.Text.Json.Serialization;
using System.Net.Http.Json;
using System.Collections.Immutable;

namespace Web.Services
{
    public class ProjectsService
    {
        public HttpClient Client { get; }
        public ProjectsService(HttpClient client)
        {
            Client = client;
        }

        public async Task<IReadOnlyCollection<string>> GetProjectSlugs()
        {
            var projects = await GetProjects();
            return projects.Select(x => x.Slug).ToImmutableArray();
        }

        public async Task<ICollection<Project>> GetProjects()
        {
            var root = await Client.GetAsync("projects.json");
            if(root.IsSuccessStatusCode && root.Content is not null)
                return await root.Content.ReadFromJsonAsync<ICollection<Project>>();

            return Array.Empty<Project>();
        }

        public struct Project
        {
            public string Name { get; set; }
            public string Slug { get; set; }
        }
    }
}
